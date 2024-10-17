namespace VStore.Application.Abstractions.QueryModel;

public abstract class PageModel
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}