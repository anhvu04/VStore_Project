using System.Collections;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Cart.Common;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;
using VStore.Domain.Enums;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Cart.Command.AddToCart;

public class AddToCartCommandHandler : ICommandHandler<AddToCartCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly ICartRepository _cartRepository;
    private readonly ICartDetailRepository _cartDetailRepository;

    public AddToCartCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork,
        IProductRepository productRepository, ICartRepository cartRepository,
        ICartDetailRepository cartDetailRepository)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _cartRepository = cartRepository;
        _cartDetailRepository = cartDetailRepository;
    }

    public async Task<Result> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.FindByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Product)));
        }

        if (product.Quantity < request.Quantity || product.Status == ProductStatus.OutOfStock)
        {
            return Result.Failure(DomainError.Product.NotEnoughQuantity);
        }

        var cart = await _cartRepository.FindSingleAsync(x => x.CustomerId == request.UserId, cancellationToken,
            x => x.CartDetails);
        if (cart == null)
        {
            cart = new Domain.Entities.Cart { CustomerId = request.UserId };
            _cartRepository.Add(cart);
        }

        var cartDetail = cart.CartDetails.FirstOrDefault(
            x => x.CartId == cart.Id && x.ProductId == product.Id);

        if (cartDetail == null)
        {
            _cartDetailRepository.Add(new CartDetail
            {
                CartId = cart.Id,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            });
        }
        else
        {
            if (cartDetail.Quantity + request.Quantity > product.Quantity)
            {
                return Result.Failure(DomainError.Product.ExceedQuantity(cartDetail.Quantity, product.Quantity));
            }

            cartDetail.Quantity += request.Quantity;
            _cartDetailRepository.Update(cartDetail);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}