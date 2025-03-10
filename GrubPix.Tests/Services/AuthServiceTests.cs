using System;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using GrubPix.Application.DTO;
using GrubPix.Application.Exceptions;
using GrubPix.Application.Interfaces;
using GrubPix.Application.Services;
using GrubPix.Domain.Entities;
using GrubPix.Domain.Interfaces.Repositories;
using GrubPix.Application.Interfaces.Services;
using GrubPix.Application.Utils;

namespace GrubPix.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<AuthService>> _mockLogger;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockJwtService = new Mock<IJwtService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<AuthService>>();

            _authService = new AuthService(
                _mockUserRepository.Object,
                _mockCustomerRepository.Object,
                _mockJwtService.Object,
                _mockEmailService.Object,
                _mockMapper.Object,
                _mockLogger.Object);
        }

        // ------------------- RegisterAsync Tests -------------------

        [Fact]
        public async Task RegisterAsync_ShouldRegisterUser_WhenRoleIsNotCustomer()
        {
            var registerDto = new RegisterDto
            {
                Username = "newuser",
                Email = "user@example.com",
                Password = "password",
                Role = "Admin"
            };

            var user = new User
            {
                Id = 1,
                Email = registerDto.Email,
                Username = registerDto.Username,
                PasswordHash = "hashed_password",
                Role = registerDto.Role
            };

            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(registerDto.Email)).ReturnsAsync((User)null);
            _mockUserRepository.Setup(repo => repo.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(user);
            _mockEmailService.Setup(service => service.SendVerificationEmail(user.Email, It.IsAny<string>()));

            _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto { Email = user.Email });

            var result = await _authService.RegisterAsync(registerDto);

            Assert.NotNull(result);
            Assert.IsType<UserDto>(result);
            Assert.Equal(registerDto.Email, result.Email);
        }

        [Fact]
        public async Task RegisterAsync_ShouldRegisterCustomer_WhenRoleIsCustomer()
        {
            var registerDto = new RegisterDto
            {
                Username = "newcustomer",
                Email = "customer@example.com",
                Password = "password",
                Role = "Customer"
            };

            var customer = new Customer
            {
                Id = 2,
                Email = registerDto.Email,
                Username = registerDto.Username,
                PasswordHash = "hashed_password",
                Role = registerDto.Role
            };

            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(registerDto.Email)).ReturnsAsync((User)null);
            _mockCustomerRepository.Setup(repo => repo.CreateCustomerAsync(It.IsAny<Customer>())).ReturnsAsync(customer);
            _mockEmailService.Setup(service => service.SendVerificationEmail(customer.Email, It.IsAny<string>()));

            _mockMapper.Setup(m => m.Map<CustomerDto>(It.IsAny<Customer>())).Returns(new CustomerDto { Email = customer.Email });

            var result = await _authService.RegisterAsync(registerDto);

            Assert.NotNull(result);
            Assert.IsType<CustomerDto>(result);
            Assert.Equal(registerDto.Email, result.Email);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenEmailAlreadyExists()
        {
            var registerDto = new RegisterDto
            {
                Email = "existing@example.com",
                Password = "password",
                Role = "Admin"
            };

            var existingUser = new User { Email = registerDto.Email };

            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(registerDto.Email)).ReturnsAsync(existingUser);

            await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(registerDto));
        }


        // ------------------- AuthenticateAsync Tests -------------------

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnUserDto_WhenValidUserCredentials()
        {
            var loginDto = new LoginDto { Email = "user@example.com", Password = "password" };
            var user = new User
            {
                Id = 1,
                Email = loginDto.Email,
                PasswordHash = PasswordHelper.HashPassword("password"),
                Username = "TestUser",
                Role = "Admin",
                IsVerified = true
            };

            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(loginDto.Email)).ReturnsAsync(user);
            _mockCustomerRepository.Setup(repo => repo.GetCustomerByEmailAsync(loginDto.Email)).ReturnsAsync((Customer)null);
            _mockJwtService.Setup(jwt => jwt.GenerateToken(user.Id, user.Username, user.Role)).Returns("mocked-jwt-token");
            _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto { Email = user.Email, Token = "mocked-jwt-token" });

            var result = await _authService.AuthenticateAsync(loginDto);

            Assert.NotNull(result);
            Assert.IsType<UserDto>(result);
            Assert.Equal("mocked-jwt-token", result.Token);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnCustomerDto_WhenValidCustomerCredentials()
        {
            var loginDto = new LoginDto { Email = "customer@example.com", Password = "password" };
            var customer = new Customer
            {
                Id = 2,
                Email = loginDto.Email,
                PasswordHash = PasswordHelper.HashPassword("password"),
                Username = "TestCustomer",
                IsVerified = true
            };

            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(loginDto.Email)).ReturnsAsync((User)null);
            _mockCustomerRepository.Setup(repo => repo.GetCustomerByEmailAsync(loginDto.Email)).ReturnsAsync(customer);
            _mockJwtService.Setup(jwt => jwt.GenerateToken(customer.Id, customer.Email, "Customer")).Returns("mocked-jwt-token");
            _mockMapper.Setup(m => m.Map<CustomerDto>(It.IsAny<Customer>())).Returns(new CustomerDto { Email = customer.Email, Token = "mocked-jwt-token" });

            var result = await _authService.AuthenticateAsync(loginDto);

            Assert.NotNull(result);
            Assert.IsType<CustomerDto>(result);
            Assert.Equal("mocked-jwt-token", result.Token);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowEmailNotVerifiedException_WhenUserIsNotVerified()
        {
            var loginDto = new LoginDto { Email = "user@example.com", Password = "password" };
            var user = new User
            {
                Id = 1,
                Email = loginDto.Email,
                PasswordHash = PasswordHelper.HashPassword("password"),
                Username = "TestUser",
                Role = "Admin",
                IsVerified = false
            };

            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(loginDto.Email)).ReturnsAsync(user);

            await Assert.ThrowsAsync<EmailNotVerifiedException>(() => _authService.AuthenticateAsync(loginDto));
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowEmailNotVerifiedException_WhenCustomerIsNotVerified()
        {
            var loginDto = new LoginDto { Email = "customer@example.com", Password = "password" };
            var customer = new Customer
            {
                Id = 2,
                Email = loginDto.Email,
                PasswordHash = PasswordHelper.HashPassword("password"),
                Username = "TestCustomer",
                IsVerified = false
            };

            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(loginDto.Email)).ReturnsAsync((User)null);
            _mockCustomerRepository.Setup(repo => repo.GetCustomerByEmailAsync(loginDto.Email)).ReturnsAsync(customer);

            await Assert.ThrowsAsync<EmailNotVerifiedException>(() => _authService.AuthenticateAsync(loginDto));
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowException_WhenCredentialsAreInvalid()
        {
            var loginDto = new LoginDto { Email = "user@example.com", Password = "wrongpassword" };

            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(loginDto.Email)).ReturnsAsync((User)null);
            _mockCustomerRepository.Setup(repo => repo.GetCustomerByEmailAsync(loginDto.Email)).ReturnsAsync((Customer)null);

            await Assert.ThrowsAsync<Exception>(() => _authService.AuthenticateAsync(loginDto));
        }


        // ------------------- VerifyEmailAsync Tests -------------------

        [Fact]
        public async Task VerifyEmailAsync_ShouldVerifyUser_WhenTokenIsValid()
        {
            string token = "valid-token";
            var user = new User { Id = 1, Email = "user@example.com", IsVerified = false, VerificationToken = token };

            _mockUserRepository.Setup(repo => repo.GetByVerificationTokenAsync(token)).ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.UpdateAsync(user)).ReturnsAsync(user);

            var result = await _authService.VerifyEmailAsync(token);

            Assert.True(result);
        }

        [Fact]
        public async Task VerifyEmailAsync_ShouldVerifyCustomer_WhenTokenIsValid()
        {
            string token = "valid-token";
            var customer = new Customer { Id = 2, Email = "customer@example.com", IsVerified = false, VerificationToken = token };

            _mockUserRepository.Setup(repo => repo.GetByVerificationTokenAsync(token)).ReturnsAsync((User)null);
            _mockCustomerRepository.Setup(repo => repo.GetByVerificationTokenAsync(token)).ReturnsAsync(customer);
            _mockCustomerRepository.Setup(repo => repo.UpdateAsync(customer)).ReturnsAsync(customer);

            var result = await _authService.VerifyEmailAsync(token);

            Assert.True(result);
        }

        [Fact]
        public async Task VerifyEmailAsync_ShouldThrowException_WhenTokenIsInvalid()
        {
            string token = "invalid-token";

            _mockUserRepository.Setup(repo => repo.GetByVerificationTokenAsync(token)).ReturnsAsync((User)null);
            _mockCustomerRepository.Setup(repo => repo.GetByVerificationTokenAsync(token)).ReturnsAsync((Customer)null);

            var result = await _authService.VerifyEmailAsync(token);

            Assert.False(result);
        }

        // ------------------- ForgotPasswordAsync Tests -------------------

        [Fact]
        public async Task ForgotPasswordAsync_ShouldReturnFalse_WhenEmailDoesNotExist()
        {
            string email = "unknown@example.com";

            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(email)).ReturnsAsync((User)null);
            _mockCustomerRepository.Setup(repo => repo.GetCustomerByEmailAsync(email)).ReturnsAsync((Customer)null);

            var result = await _authService.ForgotPasswordAsync(email);

            Assert.False(result);
        }

        [Fact]
        public async Task ForgotPasswordAsync_ShouldSendResetEmail_WhenUserExists()
        {
            string email = "user@example.com";
            var user = new User { Id = 1, Email = email, PasswordResetToken = null, ResetTokenExpiry = null };

            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(email)).ReturnsAsync(user);
            _mockEmailService.Setup(service => service.SendEmailAsync(email, It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(repo => repo.UpdateAsync(user)).ReturnsAsync(user);

            var result = await _authService.ForgotPasswordAsync(email);

            Assert.True(result);
        }

        [Fact]
        public async Task ForgotPasswordAsync_ShouldSendResetEmail_WhenCustomerExists()
        {
            string email = "customer@example.com";
            var customer = new Customer { Id = 2, Email = email, PasswordResetToken = null, ResetTokenExpiry = null };

            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(email)).ReturnsAsync((User)null);
            _mockCustomerRepository.Setup(repo => repo.GetCustomerByEmailAsync(email)).ReturnsAsync(customer);
            _mockEmailService.Setup(service => service.SendEmailAsync(email, It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(Task.CompletedTask);
            _mockCustomerRepository.Setup(repo => repo.UpdateAsync(customer)).ReturnsAsync(customer);

            var result = await _authService.ForgotPasswordAsync(email);

            Assert.True(result);
        }

        // ------------------- ResetPasswordAsync Tests -------------------

        [Fact]
        public async Task ResetPasswordAsync_ShouldResetPassword_WhenTokenIsValidForUser()
        {
            string token = "valid-reset-token";
            var user = new User { Id = 1, Email = "user@example.com", PasswordResetToken = token, ResetTokenExpiry = DateTime.UtcNow.AddHours(1) };

            _mockUserRepository.Setup(repo => repo.GetByResetTokenAsync(token)).ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.UpdateAsync(user)).ReturnsAsync(user);

            var result = await _authService.ResetPasswordAsync(token, "newpassword");

            Assert.True(result);
        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldResetPassword_WhenTokenIsValidForCustomer()
        {
            string token = "valid-reset-token";
            var customer = new Customer { Id = 2, Email = "customer@example.com", PasswordResetToken = token, ResetTokenExpiry = DateTime.UtcNow.AddHours(1) };

            _mockUserRepository.Setup(repo => repo.GetByResetTokenAsync(token)).ReturnsAsync((User)null);
            _mockCustomerRepository.Setup(repo => repo.GetByResetTokenAsync(token)).ReturnsAsync(customer);
            _mockCustomerRepository.Setup(repo => repo.UpdateAsync(customer)).ReturnsAsync(customer);

            var result = await _authService.ResetPasswordAsync(token, "newpassword");

            Assert.True(result);
        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldReturnFalse_WhenTokenIsInvalidOrExpired()
        {
            string token = "expired-reset-token";
            var user = new User { Id = 1, Email = "user@example.com", PasswordResetToken = token, ResetTokenExpiry = DateTime.UtcNow.AddHours(-1) };

            _mockUserRepository.Setup(repo => repo.GetByResetTokenAsync(token)).ReturnsAsync(user);

            var result = await _authService.ResetPasswordAsync(token, "newpassword");

            Assert.False(result);
        }

    }
}