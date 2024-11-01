using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace VStore.API.Controllers;

[ApiController]
[Route("api")]
public class FileController(ISender sender) : ApiController(sender)
{
    private static readonly string BasePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

    [HttpGet]
    [Route("files/{filePath}")]
    public IActionResult GetFile([FromRoute] string filePath)
    {
        // Decode the URL-encoded file path (handles %2F)
        var parts = filePath.Split("%2F");
        // Combine the parts to get the full path
        var path = Path.Combine(parts);
        path = Path.Combine(BasePath, path);

        if (!System.IO.File.Exists(path))
        {
            return NotFound();
        }

        // Try to determine the content type of the file
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(path, out var contentType))
        {
            // Default to "text/plain" if the file type is unknown
            contentType = "text/plain";
        }

        return PhysicalFile(path, contentType);
    }
}