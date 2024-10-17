using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.User.Command.ChangePassword;

public class ChangePasswordCommand : ICommand
{
    [JsonIgnore] public Guid UserId { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}