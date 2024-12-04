using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.CloudinaryService;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Models.CloudinaryService;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.ProductImage.Command.CreateProductImage;

public class CreateProductImageCommandHandler : ICommandHandler<CreateProductImageCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductImageRepository _productImageRepository;

    public CreateProductImageCommandHandler(IProductRepository productRepository, ICloudinaryService cloudinaryService,
        IUnitOfWork unitOfWork, IProductImageRepository productImageRepository)
    {
        _productRepository = productRepository;
        _cloudinaryService = cloudinaryService;
        _unitOfWork = unitOfWork;
        _productImageRepository = productImageRepository;
    }

    public async Task<Result> Handle(CreateProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.FindAll(x => x.Id == request.ProductId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (product == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Product)));
        }

        if (product.ProductImages.Count + request.Images.Count > 10)
        {
            return Result.Failure(DomainError.ProductImage.ExceedLimit);
        }

        var uploadTask = request.Images.Select(async image =>
        {
            var result = await _cloudinaryService.UploadImageAsync(image);
            if (result.Code == 200)
            {
                var data = JsonSerializer.Serialize(result.Data);
                var imageUrl = JsonSerializer.Deserialize<CloudinaryResponseModel>(data);
                if (imageUrl == null)
                {
                    return Result.Failure(DomainError.CommonError.ExceptionHandled(nameof(CloudinaryResponseModel)));
                }

                _productImageRepository.Add(new Domain.Entities.ProductImage
                {
                    ProductId = request.ProductId,
                    ImageUrl = imageUrl.SecureUri,
                    PublicId = imageUrl.PublicId,
                    IsActive = false
                });
                return Result.Success();
            }

            return Result.Failure(DomainError.ProductImage.UploadImageFail(image.FileName));
        });

        // Wait for all upload tasks to complete
        await Task.WhenAll(uploadTask);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}