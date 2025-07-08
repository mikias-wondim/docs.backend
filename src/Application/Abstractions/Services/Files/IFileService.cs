namespace Application.Abstractions.Services.Files;

public interface IFileService
{
    Task<string> UploadAsync(Stream content, string directory, string fileName);
    Task<Stream> GetAsync(string relativePath);
    Task<bool> DeleteAsync(string relativePath);
}
