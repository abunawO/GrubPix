
using GrubPix.Application.Services.Interfaces;

namespace GrubPix.Infrastructure.Services
{
    public class ImageStorageService : IImageStorageService
    {
        public async Task<string> UploadImageAsync(Stream imageStream, string fileName)
        {
            // Placeholder for actual image upload logic
            await Task.Delay(100); // Simulate async work
            return $"https://fakecdn.com/{fileName}";
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            // Placeholder for actual image deletion logic
            await Task.Delay(50); // Simulate async work
            return true;
        }
    }
}
