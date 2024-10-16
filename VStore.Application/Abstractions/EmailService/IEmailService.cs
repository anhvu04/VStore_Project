using VStore.Application.Models;
using VStore.Domain.Shared;

namespace VStore.Application.Abstractions.EmailService;

public interface IEmailService
{
    Task<Result> SendEmailAsync(SendMailModel model, bool isBodyHtml = false,
        CancellationToken cancellationToken = default);

    Task SendActivationEmailAsync(string to, string token, bool isVerify = true,
        CancellationToken cancellationToken = default);
}