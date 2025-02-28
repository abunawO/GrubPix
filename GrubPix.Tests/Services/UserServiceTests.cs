using System;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using GrubPix.Application.DTO;
using GrubPix.Application.Exceptions;
using GrubPix.Application.Services;
using GrubPix.Domain.Entities;
using GrubPix.Domain.Interfaces.Repositories;

namespace GrubPix.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<UserService>>();

            _userService = new UserService(
                _mockUserRepository.Object,
                _mockMapper.Object,
                _mockLogger.Object);
        }

        // ------------------- UpdateUserAsync Tests -------------------

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUser_WhenUserExists()
        {
            int userId = 1;
            var updateDto = new UpdateUserDto { Email = "updated@example.com", Username = "UpdatedUser" };
            var existingUser = new User { Id = userId, Email = "old@example.com", Username = "OldUser" };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(existingUser);
            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(updateDto.Email)).ReturnsAsync((User)null);
            _mockUserRepository.Setup(repo => repo.UpdateAsync(It.IsAny<User>())).ReturnsAsync(existingUser);

            _mockMapper.Setup(m => m.Map(updateDto, existingUser));
            _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto { Email = updateDto.Email, Username = updateDto.Username });

            var result = await _userService.UpdateUserAsync(userId, updateDto);

            Assert.NotNull(result);
            Assert.Equal(updateDto.Email, result.Email);
            Assert.Equal(updateDto.Username, result.Username);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrowException_WhenEmailAlreadyExists()
        {
            int userId = 1;
            var updateDto = new UpdateUserDto { Email = "existing@example.com" };
            var existingUser = new User { Id = userId, Email = "old@example.com" };
            var anotherUser = new User { Id = 2, Email = "existing@example.com" };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(existingUser);
            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(updateDto.Email)).ReturnsAsync(anotherUser);

            await Assert.ThrowsAsync<Exception>(() => _userService.UpdateUserAsync(userId, updateDto));
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            int userId = 1;
            var updateDto = new UpdateUserDto { Email = "updated@example.com" };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _userService.UpdateUserAsync(userId, updateDto));
        }

        // ------------------- DeleteUserAsync Tests -------------------

        [Fact]
        public async Task DeleteUserAsync_ShouldReturnTrue_WhenUserExists()
        {
            int userId = 1;
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId });
            _mockUserRepository.Setup(repo => repo.DeleteAsync(userId)).ReturnsAsync(true);

            var result = await _userService.DeleteUserAsync(userId);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            int userId = 1;
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _userService.DeleteUserAsync(userId));
        }

        // ------------------- GetByUsernameAsync Tests -------------------

        [Fact]
        public async Task GetByUsernameAsync_ShouldReturnUserDto_WhenUserExists()
        {
            string username = "testuser";
            var user = new User { Id = 1, Username = username, Email = "test@example.com" };

            _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(username)).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto { Username = user.Username, Email = user.Email });

            var result = await _userService.GetByUsernameAsync(username);

            Assert.NotNull(result);
            Assert.Equal(username, result.Username);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetByUsernameAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            string username = "unknownuser";

            _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(username)).ReturnsAsync((User)null);

            var result = await _userService.GetByUsernameAsync(username);

            Assert.Null(result);
        }

        // ------------------- GetByEmailAsync Tests -------------------

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnUserDto_WhenUserExists()
        {
            string email = "test@example.com";
            var user = new User { Id = 1, Email = email, Username = "TestUser" };

            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(email)).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto { Email = user.Email, Username = user.Username });

            var result = await _userService.GetByEmailAsync(email);

            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            string email = "unknown@example.com";

            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(email)).ReturnsAsync((User)null);

            var result = await _userService.GetByEmailAsync(email);

            Assert.Null(result);
        }

        // ------------------- GetUserByIdAsync Tests -------------------

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUserDto_WhenUserExists()
        {
            int userId = 1;
            var user = new User { Id = userId, Email = "test@example.com", Username = "TestUser" };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto { Email = user.Email, Username = user.Username });

            var result = await _userService.GetUserByIdAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            int userId = 1;

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User)null);

            var result = await _userService.GetUserByIdAsync(userId);

            Assert.Null(result);
        }
    }
}