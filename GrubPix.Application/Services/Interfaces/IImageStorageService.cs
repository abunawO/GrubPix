namespace GrubPix.Application.Services.Interfaces
{
    public interface IImageStorageService
    {
        Task<string> UploadImageAsync(Stream imageStream, string fileName);
        Task<bool> DeleteImageAsync(string imageUrl);
    }
}