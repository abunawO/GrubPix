using System.Net;
using System.Net.Mail;
using GrubPix.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace GrubPix.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendVerificationEmail(string email, string token)
        {
            try
            {
                string verificationLink = $"{_config["AppSettings:FrontendUrl"]}/verify?token={token}";

                _logger.LogInformation("Generated verification link: {Link}", verificationLink);

                string subject = "Verify Your Email - GrubPix";
                string body = $"Click the link to verify your account: <a href='{verificationLink}'>Verify Now</a>";

                _logger.LogInformation("Sending verification email to {Email}", email);
                await SendEmailAsync(email, subject, body);
                _logger.LogInformation("Verification email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send verification email to {Email}", email, ex.Message);
            }
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var apiKey = _config["Email:Password"];
            var fromEmail = _config["Email:From"];

            _logger.LogInformation("Using SendGrid API Key: {ApiKey}", apiKey);

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, "GrubPix");
            var toEmail = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, body, body);

            var response = await client.SendEmailAsync(msg);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to send email. Status Code: {response.StatusCode}");
            }
        }

    }


}