using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Net.payOS;
using Net.payOS.Types;
using VStore.Application.Abstractions.ApiService;
using VStore.Application.Abstractions.PayOsService;
using VStore.Application.Models;
using VStore.Application.Models.PayOsService;
using VStore.Application.Usecases.Checkout.Common;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Enums;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;
using ItemData = Net.payOS.Types.ItemData;

namespace VStore.Infrastructure.PayOsService;

public class PayOsService : IPayOsService
{
    private readonly PayOS _payOs;
    private readonly IConfiguration _configuration;
    private readonly PayOsLibrary _payOsLibrary;
    private readonly ILogger<PayOsService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IApiService _apiService;
    private const string PaymentInformationUrl = "https://api-merchant.payos.vn/v2/payment-requests/";

    public PayOsService(IConfiguration configuration, PayOsLibrary payOsLibrary, ILogger<PayOsService> logger,
        IServiceProvider serviceProvider, IApiService apiService)
    {
        _configuration = configuration;
        _payOsLibrary = payOsLibrary;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _apiService = apiService;
        _payOs = new PayOS(_configuration["PayOs:ClientId"]!, _configuration["PayOs:ApiKey"]!,
            _configuration["PayOs:ChecksumKey"]!);
        _payOs.confirmWebhook(_configuration["PayOs:WebhookUrl"]!);
    }

    public async Task<ApiResponseModel> CreatePaymentLink(CreatePayOsPaymentModel model)
    {
        try
        {
            var paymentData = new PaymentData(
                orderCode: model.OrderCode,
                amount: model.Amount,
                description: model.Description,
                items: model.Items.Select(i => new ItemData(i.Name, i.Quantity, i.Price)).ToList(),
                cancelUrl: _configuration["PayOs:CancelUrl"]!,
                returnUrl: _configuration["PayOs:ReturnUrl"]!,
                expiredAt: DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds()
            );
            var payment = await _payOs.createPaymentLink(paymentData);
            return new ApiResponseModel(code: 200, data: payment.checkoutUrl);
        }
        catch (Exception)
        {
            return new ApiResponseModel(code: 400, data: null);
        }
    }

    public async Task<Result<PayOsWebHookResponseModel>> VerifyPaymentWebHook(VerifyPayOsWebHookModel request,
        CancellationToken cancellationToken)
    {
        var validateSignature = await VerifyPaymentWebHook(request);
        if (validateSignature.Code != 200)
        {
            _logger.LogInformation("Invalid signature");
            return Result<PayOsWebHookResponseModel>.Success(
                new PayOsWebHookResponseModel(false, "Invalid signature"));
        }

        var response = validateSignature.Data as VerifySignatureResponseModel;
        if (response == null)
        {
            _logger.LogInformation("Invalid response");
            return Result<PayOsWebHookResponseModel>.Success(
                new PayOsWebHookResponseModel(false, "Invalid response data after verifying signature"));
        }

        var serviceProvider = _serviceProvider.CreateScope();
        var orderRepository = serviceProvider.ServiceProvider.GetRequiredService<IOrderRepository>();
        var productRepository = serviceProvider.ServiceProvider.GetRequiredService<IProductRepository>();
        var unitOfWork = serviceProvider.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var order = await orderRepository.FindAll(x => x.TransactionCode == response.OrderCode)
            .Include(x => x.OrderDetails).ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (order == null)
        {
            _logger.LogInformation("Order not found");
            return Result<PayOsWebHookResponseModel>.Success(
                new PayOsWebHookResponseModel(false, "Order not found"));
        }

        if (response.Code == "00")
        {
            _logger.LogInformation("Order {0} is paid", order.TransactionCode);
            order.Status = OrderStatus.Processing;
        }
        else
        {
            _logger.LogInformation("Order {0} is not paid", order.TransactionCode);
            order.Status = OrderStatus.Cancelled;
            foreach (var product in order.OrderDetails)
            {
                if (product.Product.Status == ProductStatus.OutOfStock)
                {
                    product.Product.Status = ProductStatus.Selling;
                }

                product.Product.Quantity += product.Quantity;
                productRepository.Update(product.Product);
            }
        }

        orderRepository.Update(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<PayOsWebHookResponseModel>.Success(new PayOsWebHookResponseModel(true, "Success update order"));
    }

    public async Task<Result<PayOsWebHookResponseModel>> CreateSignatureWebHook(CreatePayOsSignatureModel request,
        CancellationToken cancellationToken)
    {
        var response = await CreatePayOsSignatureWebHook(request);
        return Result<PayOsWebHookResponseModel>.Success(new PayOsWebHookResponseModel(true,
            response.Data!.ToString()));
    }

    public async Task<Result<PayOsWebHookResponseModel>> GetPaymentInformation(long orderCode,
        CancellationToken cancellationToken = default)
    {
        var response = await _apiService.GetAsync(PaymentInformationUrl + orderCode,
            headers =>
            {
                headers.Add("x-api-key", _configuration["PayOs:ApiKey"]!);
                headers.Add("x-client-id", _configuration["PayOs:ClientId"]!);
            });
        if (!response.IsSuccess)
        {
            return Result<PayOsWebHookResponseModel>.Failure(DomainError.PayOs.GetPaymentInfoError);
        }

        var data = JsonSerializer.Deserialize<VerifyPayOsWebHookModel>(response.Value!);
        if (data == null)
        {
            return Result<PayOsWebHookResponseModel>.Failure(DomainError.PayOs.GetPaymentInfoError);
        }

        return Result<PayOsWebHookResponseModel>.Success(new PayOsWebHookResponseModel(true, data));
    }

    public async Task<Result<PayOsWebHookResponseModel>> VerifyPaymentWebHookType(WebhookType data)
    {
        try
        {
            // var webHookData = new WebhookData(data.Data.OrderCode, data.Data.Amount, data.Data.Description,
            //     data.Data.AccountNumber, data.Data.Reference, data.Data.TransactionDateTime, data.Data.Currency,
            //     data.Data.PaymentLinkId, data.Data.Code, data.Data.Desc, data.Data.CounterAccountBankId,
            //     data.Data.CounterAccountBankName, data.Data.CounterAccountName, data.Data.CounterAccountNumber,
            //     data.Data.VirtualAccountName, data.Data.VirtualAccountNumber);
            // var webHookType = new WebhookType(data.Code, data.Desc, data.Success, webHookData, data.Signature);
            // var res = _payOs.verifyPaymentWebhookData(webHookType);
            var res = _payOs.verifyPaymentWebhookData(data);
            var serviceProvider = _serviceProvider.CreateScope();
            var orderRepository = serviceProvider.ServiceProvider.GetRequiredService<IOrderRepository>();
            var productRepository = serviceProvider.ServiceProvider.GetRequiredService<IProductRepository>();
            var unitOfWork = serviceProvider.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var order = await orderRepository.FindAll(x => x.TransactionCode == res.orderCode)
                .Include(x => x.OrderDetails).ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(cancellationToken: default);
            if (order == null)
            {
                _logger.LogInformation("Order not found");
                return Result<PayOsWebHookResponseModel>.Success(
                    new PayOsWebHookResponseModel(false, "Order not found"));
            }

            if (res.code == "00")
            {
                _logger.LogInformation("Order {0} is paid", order.TransactionCode);
                order.Status = OrderStatus.Processing;
            }
            else
            {
                _logger.LogInformation("Order {0} is not paid", order.TransactionCode);
                order.Status = OrderStatus.Cancelled;
                foreach (var product in order.OrderDetails)
                {
                    if (product.Product.Status == ProductStatus.OutOfStock)
                    {
                        product.Product.Status = ProductStatus.Selling;
                    }

                    product.Product.Quantity += product.Quantity;
                    productRepository.Update(product.Product);
                }
            }

            orderRepository.Update(order);
            await unitOfWork.SaveChangesAsync(cancellationToken: default);
            return Result<PayOsWebHookResponseModel>.Success(
                new PayOsWebHookResponseModel(true, "Success update order"));
        }
        catch (Exception)
        {
            return Result<PayOsWebHookResponseModel>.Failure(DomainError.PayOs.PayOsWebhookError);
        }
    }

    private Task<ApiResponseModel> VerifyPaymentWebHook(VerifyPayOsWebHookModel data)
    {
        try
        {
            var webhookData = JsonSerializer.Deserialize<CreatePayOsSignatureModel>(data.Data.ToString()!)
                              ?? throw new InvalidOperationException();
            AddResponseData(webhookData);
            var validateSignature =
                _payOsLibrary.ValidateSignature(_configuration["PayOS:ChecksumKey"]!, data.Signature);
            if (!validateSignature)
            {
                return Task.FromResult(new ApiResponseModel(code: 400, data: null));
            }

            return Task.FromResult(new ApiResponseModel(code: 200, data: new VerifySignatureResponseModel
            {
                Code = "00",
                OrderCode = webhookData.OrderCode
            }));
        }
        catch (Exception)
        {
            return Task.FromResult(new ApiResponseModel(code: 400, data: null));
        }
    }

    private Task<ApiResponseModel> CreatePayOsSignatureWebHook(CreatePayOsSignatureModel data)
    {
        AddResponseData(data);
        var signature = _payOsLibrary.GenerateSignature(_configuration["PayOS:ChecksumKey"]!);
        return Task.FromResult(new ApiResponseModel(code: 200, data: signature));
    }

    private void AddResponseData(CreatePayOsSignatureModel webhookData)
    {
        _payOsLibrary.ClearResponseData();
        _payOsLibrary.AddResponseData("orderCode", webhookData.OrderCode.ToString());
        _payOsLibrary.AddResponseData("amount", webhookData.Amount.ToString());
        _payOsLibrary.AddResponseData("description", webhookData.Description);
        _payOsLibrary.AddResponseData("accountNumber", webhookData.AccountNumber);
        _payOsLibrary.AddResponseData("reference", webhookData.Reference);
        _payOsLibrary.AddResponseData("transactionDateTime", webhookData.TransactionDateTime);
        _payOsLibrary.AddResponseData("currency", webhookData.Currency);
        _payOsLibrary.AddResponseData("paymentLinkId", webhookData.PaymentLinkId);
        _payOsLibrary.AddResponseData("code", webhookData.Code);
        _payOsLibrary.AddResponseData("desc", webhookData.Desc);
        _payOsLibrary.AddResponseData("counterAccountBankId", webhookData.CounterAccountBankId ?? "");
        _payOsLibrary.AddResponseData("counterAccountBankName", webhookData.CounterAccountBankName ?? "");
        _payOsLibrary.AddResponseData("counterAccountName", webhookData.CounterAccountName ?? "");
        _payOsLibrary.AddResponseData("counterAccountNumber", webhookData.CounterAccountNumber ?? "");
        _payOsLibrary.AddResponseData("virtualAccountName", webhookData.VirtualAccountName ?? "");
        _payOsLibrary.AddResponseData("virtualAccountNumber", webhookData.VirtualAccountNumber ?? "");
    }
}