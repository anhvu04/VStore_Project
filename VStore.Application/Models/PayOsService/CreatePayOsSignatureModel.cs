using System.Text.Json.Serialization;

namespace VStore.Application.Models.PayOsService;

public class CreatePayOsSignatureModel
{
    [JsonPropertyName("order_code")] public long OrderCode { get; set; }

    [JsonPropertyName("amount")] public int Amount { get; set; }

    [JsonPropertyName("description")] public string Description { get; set; }

    [JsonPropertyName("account_number")] public string AccountNumber { get; set; }

    [JsonPropertyName("reference")] public string Reference { get; set; }

    [JsonPropertyName("transaction_date_time")]
    public string TransactionDateTime { get; set; }

    [JsonPropertyName("currency")] public string Currency { get; set; }

    [JsonPropertyName("payment_link_id")] public string PaymentLinkId { get; set; }

    [JsonPropertyName("code")] public string Code { get; set; }

    [JsonPropertyName("desc")] public string Desc { get; set; }

    [JsonPropertyName("counter_account_bank_id")]
    public string? CounterAccountBankId { get; set; }

    [JsonPropertyName("counter_account_bank_name")]
    public string? CounterAccountBankName { get; set; }

    [JsonPropertyName("counter_account_name")]
    public string? CounterAccountName { get; set; }

    [JsonPropertyName("counter_account_number")]
    public string? CounterAccountNumber { get; set; }

    [JsonPropertyName("virtual_account_name")]
    public string? VirtualAccountName { get; set; }

    [JsonPropertyName("virtual_account_number")]
    public string? VirtualAccountNumber { get; set; }
}