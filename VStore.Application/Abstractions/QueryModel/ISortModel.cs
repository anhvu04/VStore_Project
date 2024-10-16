namespace VStore.Application.Abstractions.QueryModel;

public interface ISortModel
{
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; }
}