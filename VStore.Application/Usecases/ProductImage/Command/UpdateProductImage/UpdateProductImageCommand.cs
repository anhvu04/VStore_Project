using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.ProductImage.Command.UpdateProductImage;

public record UpdateProductImageCommand : ICommand
{
    [JsonIgnore] public int Id { get; init; }
    public bool IsActive { get; init; }
}