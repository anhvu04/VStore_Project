using Microsoft.AspNetCore.Http;
using VStore.Application.Models;

namespace VStore.Application.Abstractions.CloudinaryService;

public interface ICloudinaryService
{
    Task<ApiResponseModel> UploadImageAsync(IFormFile file);
    Task DeleteImageAsync(string publicId);
}