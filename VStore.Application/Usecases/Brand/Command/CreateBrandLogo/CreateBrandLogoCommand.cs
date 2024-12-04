using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.Brand.Command.CreateBrandLogo;

public record CreateBrandLogoCommand : ICommand
{
    public int Id { get; init; }
    public IFormFile Logo { get; set; }
}