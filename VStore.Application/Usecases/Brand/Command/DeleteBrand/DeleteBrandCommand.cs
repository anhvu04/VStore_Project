using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.Brand.Command.DeleteBrand;

public record DeleteBrandCommand(int Id) : ICommand
{
}