using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.Product.Command.DeleteProduct;

public record DeleteProductCommand(Guid Id) : ICommand
{
}