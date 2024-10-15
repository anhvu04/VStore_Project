namespace VStore.Application.Abstractions.QueryModel;

public interface SortModel
{
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; }
}