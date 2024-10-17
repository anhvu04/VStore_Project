using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.Category.Command.DeleteCategory;

public record DeleteCategoryCommand(int Id) : ICommand
{
}