namespace GrubPix.Application.Services.Interfaces
{
    public interface IImageStorageService
    {
        Task<string> UploadImageAsync(Stream imageStream);
        Task<bool> DeleteImageAsync(string imageUrl);
        Task<Stream> GetImageAsync(string fileName);
    }
}