using VStore.Application.Models;
using VStore.Application.Models.EmailService;
using VStore.Domain.Shared;

namespace VStore.Application.Abstractions.EmailService;

public interface IEmailService
{
    Task<Result> SendEmailAsync(SendMailModel model,
        CancellationToken cancellationToken = default);

    Task SendActivationEmailAsync(string to, string token, bool isVerify = true, bool isBodyHtml = false,
        CancellationToken cancellationToken = default);
}