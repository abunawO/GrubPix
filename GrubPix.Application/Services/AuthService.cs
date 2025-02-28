
using AutoMapper;
using GrubPix.Application.DTO;
using GrubPix.Application.Interfaces;
using GrubPix.Application.Interfaces.Services;
using GrubPix.Domain.Entities;
using GrubPix.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using GrubPix.Application.Exceptions;
using GrubPix.Application.Utils;

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

        public AuthService(
            IUserRepository userRepository,
            ICustomerRepository customerRepository,
            IJwtService jwtService,
            IEmailService emailService,
            IMapper mapper,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _jwtService = jwtService;
            _emailService = emailService;
            _mapper = mapper;
            _logger = logger;
        }

        // 🔹 Register Method
        public async Task<BaseUserDto> RegisterAsync(RegisterDto dto)
        {
            _logger.LogInformation("Registering new user with email: {Email}", dto.Email);

            if (await _userRepository.GetByEmailAsync(dto.Email) != null)
                throw new Exception("Email is already in use.");

            string verificationToken = Guid.NewGuid().ToString();
            string passwordHash = PasswordHelper.HashPassword(dto.Password);

            if (dto.Role == "Customer")
            {
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

            throw new Exception("Invalid or expired verification token.");
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
            var resetLink = $"https://grubpix.com/reset-password?token={resetToken}";
            _logger.LogWarning("Password reset requested token: {token}", resetToken);

            // Send password reset email
            await _emailService.SendEmailAsync(email, "Password Reset Request",
                $"Click <a href='{resetLink}'>here</a> to reset your password.");

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
