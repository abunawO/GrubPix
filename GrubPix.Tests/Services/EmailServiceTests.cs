using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using GrubPix.Application.Services;

namespace GrubPix.Tests.Services
{
    public class EmailServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<EmailService>> _mockLogger;
        private readonly EmailService _emailService;

        public EmailServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<EmailService>>();

            // Setup configuration mock values
            _mockConfiguration.Setup(cfg => cfg["AppSettings:FrontendUrl"])
                .Returns("https://grubpix.com");
            _mockConfiguration.Setup(cfg => cfg["Email:Password"])
                .Returns("test-api-key");
            _mockConfiguration.Setup(cfg => cfg["Email:From"])
                .Returns("test@grubpix.com");

            _emailService = new EmailService(_mockConfiguration.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task SendVerificationEmail_ShouldLogInformation()
        {
            // Arrange
            string testEmail = "user@example.com";
            string testToken = "test-token";

            // Act
            await _emailService.SendVerificationEmail(testEmail, testToken);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, type) =>
                        state.ToString().Contains("Generated verification link")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, type) =>
                        state.ToString().Contains("Sending verification email to")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task SendEmailAsync_ShouldThrowException_WhenSendFails()
        {
            // Arrange
            string to = "user@example.com";
            string subject = "Test Subject";
            string body = "Test Body";

            var mockSendGridClient = new Mock<SendGrid.SendGridClient>("invalid-api-key");

            _mockConfiguration.Setup(cfg => cfg["Email:Password"]).Returns("invalid-api-key");

            var emailService = new EmailService(_mockConfiguration.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => emailService.SendEmailAsync(to, subject, body));
        }
    }
}