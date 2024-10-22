using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Product.Common;

namespace VStore.Application.Usecases.Product.Command.CreateProduct;

public record CreateProductCommand : ProductModel, ICommand
{
    [JsonIgnore] public new Guid Id { get; init; }
}