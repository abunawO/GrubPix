using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using GrubPix.Application.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace GrubPix.Tests.Services
{
    public class S3ServiceTests
    {
        private readonly Mock<IAmazonS3> _mockS3Client;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly S3Service _s3Service;

        public S3ServiceTests()
        {
            _mockS3Client = new Mock<IAmazonS3>();
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(cfg => cfg["AWS:BucketName"]).Returns("test-bucket");
            _s3Service = new S3Service(_mockS3Client.Object, _mockConfig.Object);
        }

        [Fact]
        public async Task UploadImageAsync_ShouldReturnImageUrl()
        {
            // Arrange
            var testStream = new MemoryStream();
            _mockS3Client.Setup(s3 => s3.PutObjectAsync(It.IsAny<PutObjectRequest>(), default))
                .ReturnsAsync(new PutObjectResponse { HttpStatusCode = HttpStatusCode.OK });

            // Act
            var result = await _s3Service.UploadImageAsync(testStream);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("https://test-bucket.s3.amazonaws.com/", result);
        }

        [Fact]
        public async Task DeleteImageAsync_ShouldReturnTrue_WhenImageDeleted()
        {
            // Arrange
            _mockS3Client.Setup(s3 => s3.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), default))
                .ReturnsAsync(new DeleteObjectResponse { HttpStatusCode = HttpStatusCode.NoContent });

            // Act
            var result = await _s3Service.DeleteImageAsync("test-image.jpg");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteImageAsync_ShouldReturnFalse_WhenDeletionFails()
        {
            // Arrange
            _mockS3Client.Setup(s3 => s3.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), default))
                .ReturnsAsync(new DeleteObjectResponse { HttpStatusCode = HttpStatusCode.BadRequest });

            // Act
            var result = await _s3Service.DeleteImageAsync("test-image.jpg");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetImageAsync_ShouldReturnStream_WhenImageExists()
        {
            // Arrange
            var testStream = new MemoryStream();
            var response = new GetObjectResponse { ResponseStream = testStream };

            _mockS3Client.Setup(s3 => s3.GetObjectAsync(It.IsAny<GetObjectRequest>(), default))
                .ReturnsAsync(response);

            // Act
            var result = await _s3Service.GetImageAsync("test-image.jpg");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.CanRead);
        }
    }
}