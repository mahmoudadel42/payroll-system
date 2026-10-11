using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Payroll.Application.Common.Models
{
    public record PagedRequest
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public string? Search { get; init; }
        public string? SortBy { get; init; }
        public string SortDirection { get; init; } = "asc";

        public bool Descending => string.Equals(SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
    }
    public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize)
    {
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
    public static class QueryableExtensions
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query, int page, int pageSize, CancellationToken ct)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 200);
            var total = await query.CountAsync(ct);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return new PagedResult<T>(items, total, page, pageSize);
        }
    }
}