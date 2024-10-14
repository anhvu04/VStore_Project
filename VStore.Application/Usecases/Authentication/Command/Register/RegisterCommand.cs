using ICommand = VStore.Application.Abstractions.MediatR.ICommand;

namespace VStore.Application.Usecases.Authentication.Command.Register;

public record RegisterCommand : ICommand
{
    public string Username { get; init; }
    public string Password { get; init; }
    public string ConfirmPassword { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string PhoneNumber { get; init; }
    public DateTime DateOfBirth { get; init; }
    public int Sex { get; init; }
}