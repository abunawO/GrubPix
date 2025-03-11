
using AutoMapper;
using GrubPix.Application.DTO;
using GrubPix.Application.Interfaces;
using GrubPix.Application.Interfaces.Services;
using GrubPix.Domain.Entities;
using GrubPix.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using GrubPix.Application.Exceptions;
using GrubPix.Application.Utils;
using Microsoft.Extensions.Configuration;

namespace GrubPix.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _config;

        public AuthService(
            IUserRepository userRepository,
            ICustomerRepository customerRepository,
            IJwtService jwtService,
            IEmailService emailService,
            IMapper mapper,
            ILogger<AuthService> logger, IConfiguration config)
        {
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _jwtService = jwtService;
            _emailService = emailService;
            _mapper = mapper;
            _logger = logger;
            _config = config;
        }

        // 🔹 Register Method
        public async Task<BaseUserDto> RegisterAsync(RegisterDto dto)
        {
            _logger.LogInformation("Registering new user with email: {Email}", dto.Email);
            _logger.LogInformation("Registering new user with Role: {Email}", dto.Role);

            // if (await _userRepository.GetByEmailAsync(dto.Email) != null)
            //     throw new Exception("Email is already in use.");

            string verificationToken = Guid.NewGuid().ToString();
            string passwordHash = PasswordHelper.HashPassword(dto.Password);

            if (dto.Role == "Customer")
            {
                if (await _customerRepository.GetCustomerByEmailAsync(dto.Email) != null)
                    throw new Exception("Email is already in use by another customer.");

                var customer = new Customer
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    Role = "Customer",
                    PasswordHash = passwordHash,
                    IsVerified = false,
                    VerificationToken = verificationToken
                };

                var createdCustomer = await _customerRepository.CreateCustomerAsync(customer);
                await _emailService.SendVerificationEmail(createdCustomer.Email, createdCustomer.VerificationToken);

                return _mapper.Map<CustomerDto>(createdCustomer);
            }
            else
            {
                if (await _userRepository.GetByEmailAsync(dto.Email) != null)
                    throw new Exception("Email is already in use by another restaurant owner.");

                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    PasswordHash = passwordHash,
                    Role = dto.Role,
                    IsVerified = false,
                    VerificationToken = verificationToken
                };

                var createdUser = await _userRepository.CreateUserAsync(user);
                await _emailService.SendVerificationEmail(createdUser.Email, createdUser.VerificationToken);

                return _mapper.Map<UserDto>(createdUser);
            }
        }

        // 🔹 Login Method
        public async Task<BaseUserDto> AuthenticateAsync(LoginDto dto)
        {

            _logger.LogInformation("Authenticating user with email: {Email}", dto.Email);

            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user != null && PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash))
            {
                if (!user.IsVerified) throw new EmailNotVerifiedException();

                var token = _jwtService.GenerateToken(user.Id, user.Username, user.Role);
                var userDto = _mapper.Map<UserDto>(user);
                userDto.Token = token;
                return userDto;
            }

            var customer = await _customerRepository.GetCustomerByEmailAsync(dto.Email);
            if (customer != null && PasswordHelper.VerifyPassword(dto.Password, customer.PasswordHash))
            {
                if (!customer.IsVerified) throw new EmailNotVerifiedException();

                var token = _jwtService.GenerateToken(customer.Id, customer.Email, "Customer");
                var customerDto = _mapper.Map<CustomerDto>(customer);
                customerDto.Token = token;
                return customerDto;
            }

            _logger.LogWarning("Invalid credentials for email: {Email}", dto.Email);
            throw new Exception("Invalid email or password.");

        }

        // 🔹 Verify Email Method
        public async Task<bool> VerifyEmailAsync(string token)
        {
            _logger.LogInformation("VerifyEmailAsync user with token: {token}", token);
            var user = await _userRepository.GetByVerificationTokenAsync(token);
            if (user != null)
            {
                user.IsVerified = true;
                user.VerificationToken = null;
                await _userRepository.UpdateAsync(user);
                return true;
            }

            var customer = await _customerRepository.GetByVerificationTokenAsync(token);
            if (customer != null)
            {
                customer.IsVerified = true;
                customer.VerificationToken = null;
                await _customerRepository.UpdateAsync(customer);
                return true;
            }

            return false;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            _logger.LogInformation("ForgotPasswordAsync called for email: {Email}", email);

            // Try finding the user by email
            var user = await _userRepository.GetByEmailAsync(email);
            var customer = await _customerRepository.GetCustomerByEmailAsync(email);

            if (user == null && customer == null)
            {
                _logger.LogWarning("Password reset requested for non-existent email: {Email}", email);
                return false; // Do not reveal if email exists
            }

            string resetToken = Guid.NewGuid().ToString(); // Generate token
            DateTime expiry = DateTime.UtcNow.AddHours(1); // Token expires in 1 hour

            if (user != null)
            {
                user.PasswordResetToken = resetToken;
                user.ResetTokenExpiry = expiry;
                await _userRepository.UpdateAsync(user);
            }
            else if (customer != null)
            {
                customer.PasswordResetToken = resetToken;
                customer.ResetTokenExpiry = expiry;
                await _customerRepository.UpdateAsync(customer);
            }

            // Generate reset link
            var resetLink = $"{_config["AppSettings:FrontendUrl"]}/reset-password?token={resetToken}";

            string subject = "Reset Your Password - GrubPix";
            string body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <title>Password Reset Request - GrubPix</title>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 40px auto;
                        background: #ffffff;
                        padding: 20px;
                        border-radius: 8px;
                        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                        text-align: center;
                    }}
                    .logo {{
                        font-size: 24px;
                        font-weight: bold;
                        color: #ff6600;
                        margin-bottom: 20px;
                    }}
                    .message {{
                        font-size: 16px;
                        color: #333;
                        margin-bottom: 20px;
                    }}
                    .button {{
                        display: inline-block;
                        background-color: #ff6600;
                        color: #ffffff !important;
                        padding: 12px 24px;
                        font-size: 16px;
                        text-decoration: none;
                        border-radius: 5px;
                        margin-top: 20px;
                        font-weight: bold;
                    }}
                    .footer {{
                        font-size: 12px;
                        color: #888;
                        margin-top: 30px;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='logo'>GrubPix</div>
                    <p class='message'>
                        We received a request to reset your password. If you made this request, click the button below to reset your password. 
                        If you didn’t request a password reset, you can safely ignore this email.
                    </p>
                    <a href='{resetLink}' class='button'>Reset Password</a>
                    <p class='footer'>
                        This link will expire in 24 hours. If you need further assistance, please contact our support team.
                    </p>
                </div>
            </body>
            </html>";

            _logger.LogInformation("Sending password reset email to {Email}", email);

            _logger.LogWarning("Password reset requested token: {token}", resetToken);

            // Send password reset email
            await _emailService.SendEmailAsync(email, subject, body);

            _logger.LogInformation("Password reset email sent to {Email}", email);
            return true;
        }


        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            _logger.LogInformation("ResetPasswordAsync called for token: {Token}", token);

            // Try finding the user or customer by reset token
            var user = await _userRepository.GetByResetTokenAsync(token);
            var customer = await _customerRepository.GetByResetTokenAsync(token);

            if ((user == null || user.ResetTokenExpiry < DateTime.UtcNow) &&
                (customer == null || customer.ResetTokenExpiry < DateTime.UtcNow))
            {
                _logger.LogWarning("Invalid or expired reset token used.");
                return false;
            }

            if (user != null)
            {
                user.PasswordHash = PasswordHelper.HashPassword(newPassword);
                user.PasswordResetToken = null;
                user.ResetTokenExpiry = null;
                await _userRepository.UpdateAsync(user);
                _logger.LogInformation("Password reset successfully for user {Email}", user.Email);
            }
            else if (customer != null)
            {
                customer.PasswordHash = PasswordHelper.HashPassword(newPassword);
                customer.PasswordResetToken = null;
                customer.ResetTokenExpiry = null;
                await _customerRepository.UpdateAsync(customer);
                _logger.LogInformation("Password reset successfully for customer {Email}", customer.Email);
            }

            return true;
        }

    }
}
