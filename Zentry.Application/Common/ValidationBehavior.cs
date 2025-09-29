using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Zentry.Application.Common;

/// <summary>
/// MediatR behavior for automatic validation
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    private static readonly Action<ILogger, string, int, Exception?> LogValidationFailed =
        LoggerMessage.Define<string, int>(LogLevel.Warning, new EventId(1, "ValidationFailed"),
            "Validation failed for {RequestType} with {FailureCount} errors");

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);
        
        if (_validators.Count() > 0)
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))).ConfigureAwait(false);

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                LogValidationFailed(_logger, typeof(TRequest).Name, failures.Count, null);
                throw new ValidationException(failures);
            }
        }

        return await next().ConfigureAwait(false);
    }
}
