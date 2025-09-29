using MediatR;
using Microsoft.EntityFrameworkCore;
using Zentry.Application.Common;
using Zentry.Application.DTOs;
using Zentry.Application.Mappings;
using Zentry.Application.Interfaces;
using Zentry.Domain.Entities;

namespace Zentry.Application.Features.Tasks.Queries.GetTaskById;

/// <summary>
/// Handler for GetTaskByIdQuery
/// </summary>
public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, Result<TaskDto>>
{
    private readonly IAppDbContext _context;

    public GetTaskByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TaskDto>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken).ConfigureAwait(false);
        
        if (task is null)
        {
            return Result.NotFound<TaskDto>("Task not found", "TASK_NOT_FOUND");
        }

        return Result.Ok(task.ToDto());
    }
}
