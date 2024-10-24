using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.CartService;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.CoreHelper;
using VStore.Application.Usecases.Cart.Common;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Cart.Query.GetCartQuery;

public class GetCartQueryHandler : IQueryHandler<GetCartQuery, CartModel>
{
    private readonly ICartRepository _cartRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICartService _cartService;

    public GetCartQueryHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork, ICartService cartService)
    {
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
        _cartService = cartService;
    }

    #region ChatGPT suggestion

    public async Task<Result<CartModel>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        // check any product is deleted in the cart (because of soft delete products)
        var flag = await _cartService.RefreshCartAsync(request.UserId, cancellationToken);
        var cart = await _cartRepository.FindAll(x => x.CustomerId == request.UserId)
            .Select(cart => new
            {
                CartDetails = cart.CartDetails
                    .Select(cd => new CartDetailModel
                    {
                        ProductId = cd.ProductId,
                        ProductName = cd.Product.Name,
                        BrandName = cd.Product.Brand.Name,
                        CategoryName = cd.Product.Category.Name,
                        UnitPrice = cd.Product.SalePrice == 0 ? cd.Product.OriginalPrice : cd.Product.SalePrice,
                        Quantity = cd.Quantity,
                        TotalPrice = cd.Product.SalePrice == 0
                            ? cd.Product.OriginalPrice * cd.Quantity
                            : cd.Product.SalePrice * cd.Quantity
                    })
                    .Skip((request.Page - 1) * request.PageSize) // early pagination (query in database)
                    .Take(request.PageSize)
                    .ToList(),
                TotalAmount = cart.CartDetails.Sum(cd => cd.Product.SalePrice == 0
                    ? cd.Product.OriginalPrice * cd.Quantity
                    : cd.Product.SalePrice * cd.Quantity)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (cart == null)
        {
            var newCart = new Domain.Entities.Cart
            {
                CustomerId = request.UserId,
            };
            var cartModel = new CartModel
            {
                CartDetails =
                    new PageList<CartDetailModel>(new List<CartDetailModel>(), 0, request.Page, request.PageSize),
                TotalItems = 0,
                TotalAmount = 0
            };
            _cartRepository.Add(newCart);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<CartModel>.Success(cartModel);
        }

        var cartModelResult = new CartModel
        {
            CartDetails = new PageList<CartDetailModel>(cart.CartDetails, cart.CartDetails.Count(), request.Page,
                request.PageSize),
            TotalItems = cart.CartDetails.Count,
            TotalAmount = cart.TotalAmount
        };
        if (flag)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result<CartModel>.Success(cartModelResult);
    }

    #endregion

    #region GithubCopilot suggestion

    // public async Task<Result<CartModel>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    // {
    //     var cart = await _cartRepository.FindAll(x => x.CustomerId == request.UserId)
    //         .Select(cart => new
    //         {
    //             CartDetails = cart.CartDetails.Select(cd => new CartDetailModel
    //             {
    //                 ProductId = cd.ProductId,
    //                 ProductName = cd.Product.Name,
    //                 BrandName = cd.Product.Brand.Name,
    //                 CategoryName = cd.Product.Category.Name,
    //                 UnitPrice = cd.Product.SalePrice == 0 ? cd.Product.OriginalPrice : cd.Product.SalePrice,
    //                 Quantity = cd.Quantity,
    //                 TotalPrice = cd.Product.SalePrice == 0
    //                     ? cd.Product.OriginalPrice * cd.Quantity
    //                     : cd.Product.SalePrice * cd.Quantity
    //             }).ToList()
    //         })
    //         .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    //
    //     if (cart == null)
    //     {
    //         var newCart = new Domain.Entities.Cart
    //         {
    //             CustomerId = request.UserId,
    //         };
    //         var cartModel = new CartModel
    //         {
    //             CartDetails =
    //                 new PageList<CartDetailModel>(new List<CartDetailModel>(), 0, request.Page, request.PageSize),
    //             TotalItems = 0,
    //             TotalAmount = 0
    //         };
    //         _cartRepository.Add(newCart);
    //         await _unitOfWork.SaveChangesAsync(cancellationToken);
    //         return Result<CartModel>.Success(cartModel);
    //     }
    //
    //     var cartDetails = cart.CartDetails.AsQueryable();
    //     var totalAmount = cartDetails.Sum(x => x.TotalPrice);
    //     var cartModelResult = new CartModel
    //     {
    //         CartDetails = PageList<CartDetailModel>.CreateWithoutAsync(cartDetails, request.Page, request.PageSize),
    //         TotalItems = cartDetails.Count(),
    //         TotalAmount = totalAmount
    //     };
    //
    //     return Result<CartModel>.Success(cartModelResult);
    // }

    #endregion

    #region Tao

    // public async Task<Result<CartModel>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    // {
    //     var cart = await _cartRepository.FindAll(x => x.CustomerId == request.UserId)
    //         .Include(x => x.CartDetails)
    //         .ThenInclude(x => x.Product)
    //         .ThenInclude(x => x.Brand)
    //         .Include(x => x.CartDetails)
    //         .ThenInclude(x => x.Product)
    //         .ThenInclude(x => x.Category)
    //         .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    //
    //     if (cart == null)
    //     {
    //         var newCart = new Domain.Entities.Cart
    //         {
    //             CustomerId = request.UserId,
    //         };
    //         var cartModel = new CartModel
    //         {
    //             CartDetails =
    //                 new PageList<CartDetailModel>(new List<CartDetailModel>(), 0, request.Page, request.PageSize),
    //             TotalItems = 0,
    //             TotalAmount = 0
    //         };
    //         _cartRepository.Add(newCart);
    //         await _unitOfWork.SaveChangesAsync(cancellationToken);
    //         return Result<CartModel>.Success(cartModel);
    //     }
    //
    //     var cartDetails = cart.CartDetails.Select(x => new CartDetailModel
    //     {
    //         ProductId = x.ProductId,
    //         ProductName = x.Product.Name,
    //         BrandName = x.Product.Brand.Name,
    //         CategoryName = x.Product.Category.Name,
    //         UnitPrice = x.Product.SalePrice == 0 ? x.Product.OriginalPrice : x.Product.SalePrice,
    //         Quantity = x.Quantity,
    //         TotalPrice = x.Product.SalePrice == 0
    //             ? x.Product.OriginalPrice * x.Quantity
    //             : x.Product.SalePrice * x.Quantity
    //     }).AsQueryable();
    //     var totalAmount = cartDetails.Sum(x => x.TotalPrice);
    //     var cartModelResult = new CartModel
    //     {
    //         CartDetails = PageList<CartDetailModel>.CreateWithoutAsync(cartDetails, request.Page, request.PageSize),
    //         TotalItems = cartDetails.Count(),
    //         TotalAmount = totalAmount
    //     };
    //
    //     return Result<CartModel>.Success(cartModelResult);
    // }

    #endregion
}