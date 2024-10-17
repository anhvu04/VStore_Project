using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Category.Common;

namespace VStore.Application.Usecases.Category.Command.CreateCategory;

public record CreateCategoryCommand : CategoryModel, ICommand
{
    [JsonIgnore] public new int Id { get; set; }

    // default value for root = 0
    public new int ParentId { get; set; } = 0;
}