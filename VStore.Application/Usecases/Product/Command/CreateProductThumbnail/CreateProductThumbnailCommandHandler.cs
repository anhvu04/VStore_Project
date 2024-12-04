using VStore.Application.Abstractions.MediatR;
using VStore.Application.CoreHelper;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Product.Command.CreateProductThumbnail;

public class CreateProductThumbnailCommandHandler : ICommandHandler<CreateProductThumbnailCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductThumbnailCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreateProductThumbnailCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.FindByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Product)));
        }

        if (!Path.GetExtension(request.Thumbnail.FileName).IsValidFileExtension())
        {
            return Result.Failure(DomainError.Product.InvalidFileExtension);
        }

        // Create the folder if it doesn't exist. Ex: wwwroot/products/{productId}
        var folderPath = Path.Combine(UrlHelper.ProductThumbnailPath, product.Id.ToString());
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Delete all existing files in the folder
        var existingFiles = Directory.GetFiles(folderPath, "*.*");
        foreach (var file in existingFiles)
        {
            File.Delete(file);
        }

        // Generate the file name. Ex: {productId}-thumbnail.jpg
        var fileName = $"{request.ProductId}-thumbnail";
        // Combine the file path with the file name and extension. Ex: wwwroot/products/{productId}/{productId}-thumbnail.jpg
        var filePath = Path.Combine(folderPath, $"{fileName}{Path.GetExtension(request.Thumbnail.FileName)}");

        // Save the logo file to the directory
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.Thumbnail.CopyToAsync(stream, cancellationToken);
        }

        // Ex: brands/1/1-logo.jpg
        product.Thumbnail = filePath.Split("wwwroot")[1].Replace("\\", "/");
        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(true, true, cancellationToken);
        return Result.Success();
    }
}