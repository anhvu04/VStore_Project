namespace VStore.Application.Usecases.Category.Common;

public record CategoryModel
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public bool? IsActive { get; init; }
    public int? ParentId { get; init; }
}