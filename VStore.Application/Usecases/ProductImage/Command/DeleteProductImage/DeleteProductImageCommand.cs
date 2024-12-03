using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.ProductImage.Command.DeleteProductImage;

public record DeleteProductImageCommand(int Id) : ICommand
{
}