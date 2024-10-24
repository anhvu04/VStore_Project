using Microsoft.EntityFrameworkCore;

namespace VStore.Application.CoreHelper;

public class PageList<T>
{
    public List<T> Items { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public PageList(List<T> items, int count, int page, int pageSize)
    {
        Items = items;
        TotalCount = count;
        Page = page;
        PageSize = pageSize;
    }

    public bool HasNextPage => Page * PageSize < TotalCount;
    public bool HasPreviousPage => Page > 1;

    public static async Task<PageList<T>> CreateAsync(IQueryable<T> queryable,
        int page, int pageSize)
    {
        var totalCount = await queryable.CountAsync();
        var items = await queryable.Skip((page - 1) * pageSize)
            .Take(pageSize).ToListAsync();
        return new PageList<T>(items, totalCount, page, pageSize);
    }

    public static PageList<T> CreateWithoutAsync(IQueryable<T> queryable,
        int page, int pageSize)
    {
        var totalCount = queryable.Count();
        var items = queryable.Skip((page - 1) * pageSize)
            .Take(pageSize).ToList();
        return new PageList<T>(items, totalCount, page, pageSize);
    }
}