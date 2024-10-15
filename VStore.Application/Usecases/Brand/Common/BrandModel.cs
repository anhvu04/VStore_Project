namespace VStore.Application.Usecases.Brand.Common;

public record BrandModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Logo { get; set; }
    public bool? IsActive { get; set; }
}