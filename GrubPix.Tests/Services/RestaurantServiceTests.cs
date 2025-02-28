using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using GrubPix.Application.Interfaces;
using GrubPix.Application.Services;
using GrubPix.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.Logging;
using GrubPix.Domain.Interfaces.Repositories;
using GrubPix.Application.Services.Interfaces;
using GrubPix.Application.DTO;
using Microsoft.AspNetCore.Http;
using GrubPix.Application.Exceptions;

namespace GrubPix.Tests.Services
{
    public class RestaurantServiceTests
    {
        private readonly Mock<IRestaurantRepository> _mockRestaurantRepository;
        private readonly Mock<IImageStorageService> _mockImageStorageService;
        private readonly Mock<IMenuRepository> _mockMenuRepository;
        private readonly Mock<IMenuItemRepository> _mockMenuItemRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<RestaurantService>> _mockLogger;
        private readonly RestaurantService _restaurantService;

        public RestaurantServiceTests()
        {
            _mockRestaurantRepository = new Mock<IRestaurantRepository>();
            _mockImageStorageService = new Mock<IImageStorageService>();
            _mockMenuRepository = new Mock<IMenuRepository>();
            _mockMenuItemRepository = new Mock<IMenuItemRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<RestaurantService>>();

            _restaurantService = new RestaurantService(
                _mockRestaurantRepository.Object,
                _mockImageStorageService.Object,
                _mockMenuRepository.Object,
                _mockMenuItemRepository.Object,
                _mockLogger.Object,
                _mockUserRepository.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task GetRestaurantByIdAsync_ShouldReturnRestaurant_WhenRestaurantExists()
        {
            var restaurant = new Restaurant { Id = 1, Name = "Test Restaurant" };
            _mockRestaurantRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(restaurant);
            _mockMapper.Setup(m => m.Map<RestaurantDto>(restaurant)).Returns(new RestaurantDto { Id = 1, Name = "Test Restaurant" });

            var result = await _restaurantService.GetRestaurantByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Test Restaurant", result.Name);
        }

        [Fact]
        public async Task CreateRestaurantAsync_ShouldCreateRestaurant_WhenValidDataProvided()
        {
            var restaurantDto = new CreateRestaurantDto { Name = "New Restaurant", OwnerId = 1 };
            var restaurant = new Restaurant { Id = 1, Name = "New Restaurant", OwnerId = 1 };
            var user = new User { Id = 1, Username = "Owner" };
            var imageFile = new Mock<IFormFile>();
            imageFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(user);
            _mockImageStorageService.Setup(service => service.UploadImageAsync(It.IsAny<Stream>())).ReturnsAsync("image-url");
            _mockRestaurantRepository.Setup(repo => repo.AddAsync(It.IsAny<Restaurant>())).ReturnsAsync(restaurant);
            _mockMapper.Setup(m => m.Map<RestaurantDto>(It.IsAny<Restaurant>())).Returns(new RestaurantDto { Id = 1, Name = "New Restaurant", ImageUrl = "image-url" });

            var result = await _restaurantService.CreateRestaurantAsync(restaurantDto, imageFile.Object);

            Assert.NotNull(result);
            Assert.Equal("New Restaurant", result.Name);
            Assert.Equal("image-url", result.ImageUrl);
        }

        [Fact]
        public async Task DeleteRestaurantAsync_ShouldDeleteRestaurant_WhenRestaurantExists()
        {
            var restaurant = new Restaurant { Id = 1, Name = "Test Restaurant", ImageUrl = "image-url", Menus = new List<Menu>() };
            _mockRestaurantRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(restaurant);
            _mockRestaurantRepository.Setup(repo => repo.DeleteAsync(1)).ReturnsAsync(true);
            _mockImageStorageService.Setup(service => service.DeleteImageAsync(It.IsAny<string>())).ReturnsAsync(true);

            var result = await _restaurantService.DeleteRestaurantAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task GetRestaurantsByUserIdAsync_ShouldReturnRestaurants()
        {
            // Arrange
            var userId = 1;
            var restaurants = new List<Restaurant> { new Restaurant { Id = 1, Name = "Test Restaurant", OwnerId = userId } };
            var restaurantDtos = new List<RestaurantDto> { new RestaurantDto { Id = 1, Name = "Test Restaurant" } };

            _mockRestaurantRepository.Setup(repo => repo.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(restaurants);
            _mockMapper.Setup(m => m.Map<IEnumerable<RestaurantDto>>(restaurants)).Returns(restaurantDtos);

            // Act
            var result = await _restaurantService.GetRestaurantsByUserIdAsync(null, null, false, 1, 10, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Restaurant", result.First().Name);
        }

        [Fact]
        public async Task GetRestaurantsAsync_ShouldReturnRestaurants()
        {
            // Arrange
            var restaurants = new List<Restaurant> { new Restaurant { Id = 1, Name = "Test Restaurant" } };
            var restaurantDtos = new List<RestaurantDto> { new RestaurantDto { Id = 1, Name = "Test Restaurant" } };

            _mockRestaurantRepository.Setup(repo => repo.GetAllAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(restaurants);
            _mockMapper.Setup(m => m.Map<IEnumerable<RestaurantDto>>(restaurants)).Returns(restaurantDtos);

            // Act
            var result = await _restaurantService.GetRestaurantsAsync(null, null, false, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Restaurant", result.First().Name);
        }

        [Fact]
        public async Task UpdateRestaurantAsync_ShouldUpdateRestaurant()
        {
            // Arrange
            var restaurantDto = new UpdateRestaurantDto { Name = "Updated Restaurant", Address = "New Address", Description = "Updated Description" };
            var restaurant = new Restaurant { Id = 1, Name = "Old Restaurant", Address = "Old Address", Description = "Old Description" };

            _mockRestaurantRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(restaurant);
            _mockRestaurantRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Restaurant>())).ReturnsAsync((Restaurant r) => r);
            _mockMapper.Setup(m => m.Map<RestaurantDto>(It.IsAny<Restaurant>())).Returns(new RestaurantDto
            {
                Id = 1,
                Name = restaurantDto.Name,
                Address = restaurantDto.Address,
                Description = restaurantDto.Description
            });

            // Act
            var result = await _restaurantService.UpdateRestaurantAsync(1, restaurantDto, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(restaurantDto.Name, result.Name);
            Assert.Equal(restaurantDto.Address, result.Address);
            Assert.Equal(restaurantDto.Description, result.Description);
        }

        [Fact]
        public async Task UpdateRestaurantAsync_ShouldThrowException_WhenRestaurantNotFound()
        {
            // Arrange
            _mockRestaurantRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Restaurant)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _restaurantService.UpdateRestaurantAsync(1, new UpdateRestaurantDto(), null));
        }
    }
}