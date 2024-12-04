using VStore.Application.Abstractions.MediatR;
using VStore.Application.CoreHelper;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Brand.Command.CreateBrandLogo;

public class CreateBrandLogoCommandHandler : ICommandHandler<CreateBrandLogoCommand>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBrandLogoCommandHandler(IBrandRepository brandRepository, IUnitOfWork unitOfWork)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreateBrandLogoCommand request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.FindByIdAsync(request.Id, cancellationToken);
        if (brand == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Brand)));
        }

        if (!Path.GetExtension(request.Logo.FileName).IsValidFileExtension())
        {
            return Result.Failure(DomainError.Brand.InvalidFileExtension);
        }

        // Create the folder if it doesn't exist. Ex: wwwroot/brands/1
        var folderPath = Path.Combine(UrlHelper.BrandLogoPath, brand.Id.ToString());
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

        // Generate the file name. Ex: 1-logo.jpg
        var fileName = $"{request.Id}-logo";
        // Combine the file path with the file name and extension. Ex: wwwroot/brands/1/1-logo.jpg
        var filePath = Path.Combine(folderPath, $"{fileName}{Path.GetExtension(request.Logo.FileName)}");

        // Save the logo file to the directory
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.Logo.CopyToAsync(stream, cancellationToken);
        }

        // Ex: brands/1/1-logo.jpg
        brand.Logo = filePath.Split("wwwroot")[1].Replace("\\", "/");
        _brandRepository.Update(brand);
        await _unitOfWork.SaveChangesAsync(true, true, cancellationToken);
        return Result.Success();
    }
}