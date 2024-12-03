using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.ProductImage.Command.CreateProductImage;

public record CreateProductImageCommand : ICommand
{
    public Guid ProductId { get; set; }
    public List<IFormFile> Images { get; set; }
}