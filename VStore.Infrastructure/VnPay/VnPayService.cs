using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VStore.Application.Abstractions.VNPayService;
using VStore.Application.Models;
using VStore.Application.Models.VnPayService;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;
using VStore.Infrastructure.SignalR.PresenceHub;

namespace VStore.Infrastructure.VnPay;

public class VnPayService : IVnPayService
{
    private readonly VnPayLibrary _vnpay;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly PresenceTracker _presenceTracker;
    private readonly IHubContext<PresenceHub> _presenceHub;

    public VnPayService(IConfiguration configuration, VnPayLibrary vnpay, IServiceProvider serviceProvider,
        PresenceTracker presenceTracker, IHubContext<PresenceHub> presenceHub)
    {
        _configuration = configuration;
        _vnpay = vnpay;
        _serviceProvider = serviceProvider;
        _presenceTracker = presenceTracker;
        _presenceHub = presenceHub;
        _vnpay.AddRequestData("vnp_TmnCode", _configuration["VnPay:TmnCode"]!);
        _vnpay.AddRequestData("vnp_Version", _configuration["VnPay:Version"]!);
        _vnpay.AddRequestData("vnp_Command", _configuration["VnPay:Command"]!);
        _vnpay.AddRequestData("vnp_CurrCode", _configuration["VnPay:CurrCode"]!);
        _vnpay.AddRequestData("vnp_Locale", _configuration["VnPay:Locale"]!);
        _vnpay.AddRequestData("vnp_ReturnUrl", _configuration["VnPay:ReturnUrl"]!);
    }

    public ApiResponseModel CreatePaymentLink(CreateVnPayPaymentModel model)
    {
        try
        {
            // Clear all request data before adding new data
            _vnpay.ClearRequestSpecificData();
            _vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());

            var vnTimeZoneOffset = new TimeSpan(7, 0, 0); // UTC+7 offset
            var createDate = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero).ToOffset(vnTimeZoneOffset);
            var expireDate = createDate.AddMinutes(15);

            _vnpay.AddRequestData("vnp_CreateDate", createDate.ToString("yyyyMMddHHmmss"));
            _vnpay.AddRequestData("vnp_ExpireDate", expireDate.ToString("yyyyMMddHHmmss"));
            _vnpay.AddRequestData("vnp_OrderInfo", model.OrderInfo);
            _vnpay.AddRequestData("vnp_OrderType", model.OrderType);
            _vnpay.AddRequestData("vnp_TxnRef", model.TxnRef.ToString());
            _vnpay.AddRequestData("vnp_IpAddr", model.IpAddr);
            string paymentUrl =
                _vnpay.CreateRequestUrl(_configuration["VnPay:PaymentUrl"]!, _configuration["VnPay:HashSecret"]!);
            return new ApiResponseModel
            {
                Code = 200,
                Data = paymentUrl
            };
        }
        catch (Exception e)
        {
            return new ApiResponseModel
            {
                Code = 400,
                Message = e.Message
            };
        }
    }

    public async Task<VnPayIpnResponse> VerifyIpnPayment(IQueryCollection request)
    {
        if (request.Any())
        {
            _vnpay.ClearResponseData();
            foreach (var vnp in request.Where(x
                         => x.Key.StartsWith("vnp_") && !string.IsNullOrEmpty(x.Value)))
            {
                _vnpay.AddResponseData(vnp.Key, vnp.Value + "");
            }

            var vnpSecureHash = request["vnp_SecureHash"];
            var isValidSecureHash = _vnpay.ValidateSignature(vnpSecureHash!, _configuration["VnPay:HashSecret"]!);
            if (!isValidSecureHash)
            {
                return new VnPayIpnResponse
                {
                    RspCode = "97", // invalid signature
                    Message = "Invalid signature"
                };
            }

            var serviceProvider = _serviceProvider.CreateScope();
            var unitOfWork = serviceProvider.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var orderRepository = serviceProvider.ServiceProvider
                .GetRequiredService<IOrderRepository>();
            var orderLogRepository = serviceProvider.ServiceProvider.GetRequiredService<IOrderLogRepository>();
            var productRepository = serviceProvider.ServiceProvider.GetRequiredService<IProductRepository>();

            var orderCode = long.Parse(_vnpay.GetResponseData("vnp_TxnRef"));
            var order = await orderRepository.FindAll(x => x.TransactionCode == orderCode)
                .Include(x => x.OrderDetails)
                .ThenInclude(x => x.Product).FirstOrDefaultAsync();
            if (order == null)
            {
                return new VnPayIpnResponse
                {
                    RspCode = "01", // order not found
                    Message = "Order not found"
                };
            }

            if (order.Status != Domain.Enums.OrderStatus.Pending)
            {
                return new VnPayIpnResponse
                {
                    RspCode = "02", // order already confirmed
                    Message = "Order already confirmed"
                };
            }

            var amount = int.Parse(_vnpay.GetResponseData("vnp_Amount")) / 100;
            if (order.TotalAmount != amount)
            {
                return new VnPayIpnResponse
                {
                    RspCode = "04", // invalid amount
                    Message = "invalid amount"
                };
            }

            var responseCode = _vnpay.GetResponseData("vnp_ResponseCode");
            var transactionStatus = _vnpay.GetResponseData("vnp_TransactionStatus");
            if (responseCode == "00" && transactionStatus == "00")
            {
                order.Status = Domain.Enums.OrderStatus.Processing;
                order.OrderLogs.Add(new OrderLog
                {
                    // OrderId = order.Id,
                    Status = Domain.Enums.OrderStatus.Processing,
                    CreatedDate = DateTime.UtcNow
                });
            }
            else
            {
                order.Status = Domain.Enums.OrderStatus.Cancelled;
                order.OrderLogs.Add(new OrderLog
                {
                    // OrderId = order.Id,
                    Status = Domain.Enums.OrderStatus.Cancelled,
                    CreatedDate = DateTime.UtcNow
                });
                foreach (var product in order.OrderDetails)
                {
                    product.Product.Quantity += product.Quantity;
                    if (product.Product.Status == Domain.Enums.ProductStatus.OutOfStock)
                    {
                        product.Product.Status = Domain.Enums.ProductStatus.Selling;
                    }

                    productRepository.Update(product.Product);
                }
            }

            orderRepository.Update(order);
            await unitOfWork.SaveChangesAsync(true, true);

            // Add SignalR notification
            var connectionId = await _presenceTracker.GetConnectionsForUser(order.CustomerId);
            if (connectionId != null)
            {
                await _presenceHub.Clients.Clients(connectionId).SendAsync("OrderUpdated", order.Status);
            }

            return new VnPayIpnResponse
            {
                RspCode = "00", // success
                Message = "Confirm Success"
            };
        }

        return new VnPayIpnResponse
        {
            RspCode = "99", // invalid request
            Message = "Input data required"
        };
    }
}