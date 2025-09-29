using MediatR;
using Microsoft.EntityFrameworkCore;
using Zentry.Application.Common;
using Zentry.Application.DTOs;
using Zentry.Application.Mappings;
using Zentry.Application.Interfaces;
using Zentry.Domain.Entities;

namespace Zentry.Application.Features.Tasks.Queries.GetTasks;

/// <summary>
/// Handler for GetTasksQuery
/// </summary>
public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, Result<PagedResult<TaskDto>>>
{
    private readonly IAppDbContext _context;

    public GetTasksQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<TaskDto>>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var queryable = _context.Tasks.AsQueryable();

        // Apply filters
        if (request.IsDone.HasValue)
        {
            queryable = queryable.Where(t => t.IsDone == request.IsDone.Value);
        }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchTerm = request.Search.Trim();
                queryable = queryable.Where(t => 
                    t.Title.Contains(searchTerm) || 
                    (t.Description != null && t.Description.Contains(searchTerm)));
            }

            if (request.CategoryId.HasValue)
            {
                queryable = queryable.Where(t => t.CategoryId == request.CategoryId.Value);
            }

        // Get total count
        var totalCount = await queryable.CountAsync(cancellationToken).ConfigureAwait(false);

        // Apply sorting and pagination
        var skip = (request.Page - 1) * request.PageSize;
        var items = await queryable
            .Include(t => t.Category)
            .OrderBy(t => t.IsDone) // Tamamlanmamış görevler önce
            .ThenByDescending(t => t.UpdatedAtUtc) // Sonra güncelleme tarihine göre
            .Skip(skip)
            .Take(request.PageSize)
            .Select(t => t.ToDto())
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        var result = new PagedResult<TaskDto>(items, totalCount, request.Page, request.PageSize);

        return Result.Ok(result);
    }
}
