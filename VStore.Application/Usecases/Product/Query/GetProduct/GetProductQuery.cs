using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Product.Common;

namespace VStore.Application.Usecases.Product.Query.GetProduct;

public record GetProductQuery(Guid Id) : IQuery<ProductResponseModel>
{
}