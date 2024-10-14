using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using VStore.Application.Abstractions.EmailService;
using VStore.Application.Models;
using VStore.Domain.Shared;
using VStore.Infrastructure.DependencyInjection.Options;

namespace VStore.Infrastructure.EmailService;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private const string VerifyLink = "https://localhost:5000/verify?token=";
    private const string ResetPasswordLink = "https://localhost:5000/reset-password?token=";

    public EmailService(IOptionsMonitor<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.CurrentValue;
    }

    public async Task<Result> SendEmailAsync(SendMailModel model, bool isBodyHtml = false,
        CancellationToken cancellationToken = default)
    {
        var from = _emailSettings.FromEmailAddress;
        var displayName = _emailSettings.FromDisplayName;
        var mailMessage = new MailMessage
        {
            From = new MailAddress(from, displayName),
            To = { model.To },
            Subject = model.Subject,
            Body = model.Body,
            IsBodyHtml = isBodyHtml
        };
        var smtp = new SmtpClient
        {
            Host = _emailSettings.Smtp.Host,
            Port = _emailSettings.Smtp.Port,
            Credentials = new NetworkCredential(_emailSettings.Smtp.EmailAddress, _emailSettings.Smtp.Password),
            EnableSsl = _emailSettings.Smtp.EnableSsl
        };
        await smtp.SendMailAsync(mailMessage, cancellationToken);
        return Result.Success();
    }

    public async Task SendActivationEmailAsync(string to, string token, bool isVerify,
        CancellationToken cancellationToken = default)
    {
        var model = new SendMailModel
        {
            To = to,
            Subject = isVerify ? "Verify your email" : "Reset your password",
            Body = isVerify ? VerifyLink + token : ResetPasswordLink + token
        };
        await SendEmailAsync(model, false, cancellationToken);
    }
}