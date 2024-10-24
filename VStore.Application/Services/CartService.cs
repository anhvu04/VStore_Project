using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.CartService;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;

namespace VStore.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartDetailRepository _cartDetailRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CartService(ICartRepository cartRepository, IUnitOfWork unitOfWork,
        ICartDetailRepository cartDetailRepository)
    {
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
        _cartDetailRepository = cartDetailRepository;
    }

    public async Task<bool> RefreshCartAsync(Guid userId, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.FindAll(x => x.CustomerId == userId)
            .IgnoreQueryFilters()
            .Select(x => x.CartDetails.Where(y => y.Product.IsDeleted))
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (cart == null)
        {
            return false;
        }

        _cartDetailRepository.RemoveRange(cart);
        return true;
    }
}