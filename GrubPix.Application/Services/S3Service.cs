using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using GrubPix.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GrubPix.Application.Services
{
    public class S3Service : IImageStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3Service(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _bucketName = configuration["AWS:BucketName"];
        }

        public async Task<string> UploadImageAsync(Stream imageStream)
        {
            var fileName = Guid.NewGuid().ToString(); // Generate unique filename

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                InputStream = imageStream,
                AutoCloseStream = true
            };

            await _s3Client.PutObjectAsync(request);

            return GetImageUrl(fileName); // Return full image URL
        }

        private string GetImageUrl(string fileName)
        {
            return $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = imageUrl
            };

            var response = await _s3Client.DeleteObjectAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
        }

        public async Task<Stream> GetImageAsync(string fileName)
        {
            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName
            };

            using (var response = await _s3Client.GetObjectAsync(request))
            using (var responseStream = response.ResponseStream)
            {
                var memoryStream = new MemoryStream();
                await responseStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                return memoryStream;
            }
        }
    }
}
