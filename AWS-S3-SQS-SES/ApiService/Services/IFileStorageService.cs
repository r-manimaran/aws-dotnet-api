namespace ApiService.Services;

public interface IFileStorageService
{
    Task DeleteFileAsync(string fileName);
    Task<Stream> DownloadFileAsync(string fileName);
    Task<string> UploadFileAsync(IFormFile file, string fileName);
}