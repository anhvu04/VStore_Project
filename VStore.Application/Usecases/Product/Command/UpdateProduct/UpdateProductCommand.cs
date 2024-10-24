using System.Text.Json.Serialization;
using VStore.Application.Usecases.Product.Common;
using ICommand = VStore.Application.Abstractions.MediatR.ICommand;

namespace VStore.Application.Usecases.Product.Command.UpdateProduct;

public record UpdateProductCommand : ProductModel, ICommand
{
    [JsonIgnore] public new Guid Id { get; set; }
}