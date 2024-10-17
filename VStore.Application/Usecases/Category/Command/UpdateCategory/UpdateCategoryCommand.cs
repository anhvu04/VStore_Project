using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Category.Common;

namespace VStore.Application.Usecases.Category.Command.UpdateCategory;

public record UpdateCategoryCommand : CategoryModel, ICommand
{
    [JsonIgnore] public new int Id { get; init; }
    public new int ParentId { get; init; } = 0;
}