using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.Order.Command.PayOsWebHook;

public record PayOsWebHookCommand : ICommand<string>
{
    public required string Code { get; set; }
    public required string Desc { get; set; }
    public required bool Success { get; set; }
    public required WebhookData Data { get; set; }
    public required string Signature { get; set; }
}

public record WebhookData(
    long orderCode,
    int amount,
    string description,
    string accountNumber,
    string reference,
    string transactionDateTime,
    string currency,
    string paymentLinkId,
    string code,
    string desc,
    string? counterAccountBankId,
    string? counterAccountBankName,
    string? counterAccountName,
    string? counterAccountNumber,
    string? virtualAccountName,
    string virtualAccountNumber);