using Application.Abstractions.Services.Files;

namespace Infrastructure.Services.Files;

public class LocalFileService : IFileService
{
    private readonly string _uploadsRoot;

    public LocalFileService()
    {
        _uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        Directory.CreateDirectory(_uploadsRoot);
    }

    public async Task<string> UploadAsync(Stream content, string directory, string fileName)
    {
        string targetDir = Path.Combine(_uploadsRoot, directory);

        Directory.CreateDirectory(targetDir);

        string fullPath = Path.Combine(targetDir, fileName);
        await using var fs = new FileStream(fullPath, FileMode.Create);
        await content.CopyToAsync(fs);

        return Path.Combine("uploads", directory, fileName).Replace("\\", "/");
    }

    public Task<Stream> GetAsync(string relativePath)
    {
        string fullPath = Path.Combine(_uploadsRoot, relativePath);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("Requested file not found.", relativePath);
        }

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult(stream);
    }

    public Task<bool> DeleteAsync(string relativePath)
    {
        string fullPath = Path.Combine(_uploadsRoot, relativePath);
        if (!File.Exists(fullPath))
        {
            return Task.FromResult(false);
        }

        File.Delete(fullPath);
        return Task.FromResult(true);
    }
}
