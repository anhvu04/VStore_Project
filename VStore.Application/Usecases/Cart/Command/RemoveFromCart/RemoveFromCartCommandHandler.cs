using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.CartService;
using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Cart.Command.RemoveFromCart;

public class RemoveFromCartCommandHandler : ICommandHandler<RemoveFromCartCommand>
{
    private readonly ICartRepository _cartRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICartDetailRepository _cartDetailRepository;
    private readonly ICartService _cartService;

    public RemoveFromCartCommandHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork,
        ICartDetailRepository cartDetailRepository, ICartService cartService)
    {
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
        _cartDetailRepository = cartDetailRepository;
        _cartService = cartService;
    }

    public async Task<Result> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        if (request.ProductIds.Count == 0)
        {
            return Result.Failure(DomainError.Cart.EmptyProductIds);
        }

        await _cartService.RefreshCartAsync(request.UserId, cancellationToken);
        var cart = await _cartRepository.FindAll(x => x.CustomerId == request.UserId)
            .Include(c => c.CartDetails)
            .FirstOrDefaultAsync(cancellationToken);
        if (cart == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Cart)));
        }

        var invalidProduct = request.ProductIds.Except(cart.CartDetails.Select(cd => cd.ProductId)).ToList();
        if (invalidProduct.Count != 0)
        {
            return Result.Failure(DomainError.Cart.ProductNotExistInCart(invalidProduct));
        }

        var products = cart.CartDetails.Where(cd => request.ProductIds.Contains(cd.ProductId));
        _cartDetailRepository.RemoveRange(products);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}