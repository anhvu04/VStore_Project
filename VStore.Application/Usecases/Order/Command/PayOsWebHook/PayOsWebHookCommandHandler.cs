using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Abstractions.PayOsService;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Enums;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Order.Command.PayOsWebHook;

public class PayOsWebHookCommandHandler : ICommandHandler<PayOsWebHookCommand, string>
{
    private readonly IPayOsService _payOsService;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<PayOsWebHookCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public PayOsWebHookCommandHandler(IPayOsService payOsService, IOrderRepository orderRepository,
        ILogger<PayOsWebHookCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _payOsService = payOsService;
        _orderRepository = orderRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(PayOsWebHookCommand request, CancellationToken cancellationToken)
    {
        var validateSignature = await _payOsService.VerifyPaymentWebHook(request);
        if (validateSignature.Code != 200)
        {
            _logger.LogInformation("Invalid signature");
            return Result<string>.Failure(DomainError.Checkout.PayOsWebhookError);
        }

        var order = await _orderRepository.FindAll(x => x.TransactionCode == request.Data.orderCode)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (order == null)
        {
            _logger.LogInformation("Order not found");
            return Result<string>.Success("Order not found");
        }

        if (request.Data.code == "00")
        {
            _logger.LogInformation("Order {0} is paid", order.TransactionCode);
            order.Status = OrderStatus.Processing;
        }
        else
        {
            _logger.LogInformation("Order {0} is not paid", order.TransactionCode);
            order.Status = OrderStatus.Canceled;
        }

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<string>.Success("Success");
    }
}