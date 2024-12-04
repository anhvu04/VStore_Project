using Microsoft.AspNetCore.Http;
using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.Product.Command.CreateProductThumbnail;

public record CreateProductThumbnailCommand : ICommand
{
    public Guid ProductId { get; init; }
    public IFormFile Thumbnail { get; init; }
}