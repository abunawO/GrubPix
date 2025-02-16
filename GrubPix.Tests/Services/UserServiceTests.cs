using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using GrubPix.Application.Services;
using GrubPix.Application.Interfaces.Services;
using GrubPix.Domain.Entities;
using GrubPix.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System;
using GrubPix.Application.DTO;
using GrubPix.Application.Exceptions;

namespace GrubPix.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockJwtService = new Mock<IJwtService>();
            _mockLogger = new Mock<ILogger<UserService>>();

            _userService = new UserService(_mockUserRepo.Object, _mockMapper.Object, _mockJwtService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnUser_WhenSuccessful()
        {
            // Arrange
            var dto = new RegisterDto { Username = "testuser", Email = "test@example.com", Password = "password123" };
            var user = new User { Id = 1, Username = "testuser", Email = "test@example.com", Role = "RestaurantOwner" };

            _mockUserRepo.Setup(repo => repo.GetByEmailAsync(dto.Email)).ReturnsAsync((User)null);
            _mockUserRepo.Setup(repo => repo.AddAsync(It.IsAny<User>())).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto { Id = 1, Username = "testuser", Email = "test@example.com" });

            // Act
            var result = await _userService.RegisterAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Username.Should().Be("testuser");
            result.Email.Should().Be("test@example.com");
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnUser_WhenCredentialsAreValid()
        {
            // Arrange
            var dto = new LoginDto { Email = "test@example.com", Password = "password123" };
            var user = new User { Id = 1, Username = "testuser", Email = "test@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"), Role = "RestaurantOwner" };

            _mockUserRepo.Setup(repo => repo.GetByEmailAsync(dto.Email)).ReturnsAsync(user);
            _mockJwtService.Setup(jwt => jwt.GenerateToken(user.Id, user.Username, user.Role)).Returns("fake-jwt-token");

            _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto { Id = 1, Username = "testuser", Email = "test@example.com", Token = "fake-jwt-token" });

            // Act
            var result = await _userService.AuthenticateAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Username.Should().Be("testuser");
            result.Email.Should().Be("test@example.com");
            result.Token.Should().Be("fake-jwt-token");
        }

        [Fact]
        public async Task GetByUsernameAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testuser", Email = "test@example.com" };

            _mockUserRepo.Setup(repo => repo.GetByUsernameAsync("testuser")).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto { Id = 1, Username = "testuser", Email = "test@example.com" });

            // Act
            var result = await _userService.GetByUsernameAsync("testuser");

            // Assert
            result.Should().NotBeNull();
            result.Username.Should().Be("testuser");
            result.Email.Should().Be("test@example.com");
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testuser", Email = "test@example.com" };

            _mockUserRepo.Setup(repo => repo.GetByEmailAsync("test@example.com")).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto { Id = 1, Username = "testuser", Email = "test@example.com" });

            // Act
            var result = await _userService.GetByEmailAsync("test@example.com");

            // Assert
            result.Should().NotBeNull();
            result.Username.Should().Be("testuser");
            result.Email.Should().Be("test@example.com");
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var dto = new UpdateUserDto { Email = "new@example.com" };

            _mockUserRepo.Setup(repo => repo.GetByEmailAsync(dto.Email)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _userService.UpdateUserAsync(1, dto));
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldCallRepository_WhenUserExists()
        {
            // Arrange
            var existingUser = new User { Id = 1, Username = "testuser", Email = "test@example.com" };

            _mockUserRepo.Setup(repo => repo.GetByIdAsync(existingUser.Id)).ReturnsAsync(existingUser);
            _mockUserRepo.Setup(repo => repo.DeleteAsync(existingUser.Id)).ReturnsAsync(true);

            // Act
            var result = await _userService.DeleteUserAsync(1);

            // Assert
            result.Should().BeTrue();
            _mockUserRepo.Verify(repo => repo.DeleteAsync(existingUser.Id), Times.Once);
        }
    }
}
