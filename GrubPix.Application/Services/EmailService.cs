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
                string body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <title>Verify Your Email - GrubPix</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
                        .container {{ max-width: 600px; margin: 40px auto; background: #ffffff; padding: 20px; border-radius: 8px;
                                    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1); text-align: center; }}
                        .logo {{ font-size: 24px; font-weight: bold; color: #ff6600; margin-bottom: 20px; }}
                        .message {{ font-size: 16px; color: #333; margin-bottom: 20px; }}
                        .button {{ display: inline-block; background-color: #ff6600; color: #ffffff !important; padding: 12px 24px;
                                font-size: 16px; text-decoration: none; border-radius: 5px; margin-top: 20px; font-weight: bold; }}
                        .button:link, .button:visited, .button:hover, .button:active {{
                            color: #ffffff !important;
                            text-decoration: none;
                        }}
                        .footer {{ font-size: 12px; color: #888; margin-top: 30px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='logo'>GrubPix</div>
                        <p class='message'>
                            Thank you for signing up with GrubPix! Please verify your email address to activate your account.
                        </p>
                        <a href='{verificationLink}' class='button'>Verify Now</a>
                        <p class='footer'>
                            If you did not sign up for GrubPix, you can ignore this email. This link will expire in 24 hours.
                        </p>
                    </div>
                </body>
                </html>";


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