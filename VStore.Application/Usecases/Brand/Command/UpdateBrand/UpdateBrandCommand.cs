using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Brand.Common;

namespace VStore.Application.Usecases.Brand.Command.UpdateBrand;

public record UpdateBrandCommand : BrandModel, ICommand
{
    [JsonIgnore] public new int Id { get; set; }
}