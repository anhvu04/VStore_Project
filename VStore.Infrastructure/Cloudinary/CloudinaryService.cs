using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using VStore.Application.Abstractions.CloudinaryService;
using VStore.Application.Models;

namespace VStore.Infrastructure.Cloudinary;

public class CloudinaryService : ICloudinaryService
{
    private readonly CloudinaryDotNet.Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration configuration)
    {
        _cloudinary = new CloudinaryDotNet.Cloudinary(new Account(
            configuration["CloudinarySettings:CloudName"],
            configuration["CloudinarySettings:ApiKey"],
            configuration["CloudinarySettings:ApiSecret"]
        ));
    }

    public async Task<ApiResponseModel> UploadImageAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();
        if (file.Length > 0)
        {
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
            };
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return new ApiResponseModel(200, uploadResult);
    }

    public async Task DeleteImageAsync(string publicId)
    {
        await _cloudinary.DestroyAsync(new DeletionParams(publicId));
    }
}