using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using GrubPix.Application.Interfaces;
using GrubPix.Application.Services;
using GrubPix.Domain.Entities;
using GrubPix.Application.DTO;
using AutoMapper;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using GrubPix.Domain.Interfaces.Repositories;
using GrubPix.Application.Services.Interfaces;
using GrubPix.Application.Exceptions;

namespace GrubPix.Tests.Services
{
    public class MenuItemServiceTests
    {
        private readonly Mock<IMenuItemRepository> _mockMenuItemRepository;
        private readonly Mock<IImageStorageService> _mockImageStorageService;
        private readonly Mock<ILogger<MenuItemService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly MenuItemService _menuItemService;

        public MenuItemServiceTests()
        {
            _mockMenuItemRepository = new Mock<IMenuItemRepository>();
            _mockImageStorageService = new Mock<IImageStorageService>();
            _mockLogger = new Mock<ILogger<MenuItemService>>();
            _mockMapper = new Mock<IMapper>();

            _menuItemService = new MenuItemService(
                _mockMenuItemRepository.Object,
                _mockImageStorageService.Object,
                _mockLogger.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task GetAllMenuItemsAsync_ShouldReturnMappedMenuItems()
        {
            var menuItems = new List<MenuItem> { new MenuItem { Id = 1, Name = "Burger" } };
            _mockMenuItemRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(menuItems);
            _mockMapper.Setup(m => m.Map<IEnumerable<MenuItemDto>>(menuItems)).Returns(
                new List<MenuItemDto> { new MenuItemDto { Id = 1, Name = "Burger" } }
            );

            var result = await _menuItemService.GetAllMenuItemsAsync();

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Burger", result.First().Name);
        }

        [Fact]
        public async Task GetMenuItemByIdAsync_ShouldReturnMenuItem_WhenIdIsValid()
        {
            var menuItem = new MenuItem { Id = 1, Name = "Pizza" };
            _mockMenuItemRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(menuItem);
            _mockMapper.Setup(m => m.Map<MenuItemDto>(menuItem)).Returns(new MenuItemDto { Id = 1, Name = "Pizza" });

            var result = await _menuItemService.GetMenuItemByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Pizza", result.Name);
        }

        [Fact]
        public async Task GetMenuItemByIdAsync_ShouldThrowException_WhenIdIsInvalid()
        {
            _mockMenuItemRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((MenuItem)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _menuItemService.GetMenuItemByIdAsync(1));
        }

        [Fact]
        public async Task CreateMenuItemAsync_ShouldCreateMenuItem_WithImages()
        {
            var createDto = new CreateMenuItemDto { Name = "Sushi", MenuId = 1 };
            var menuItem = new MenuItem { Id = 1, Name = "Sushi", MenuId = 1 };
            var images = new List<IFormFile> { new FormFile(Stream.Null, 0, 0, "image", "image.jpg") };

            var savedMenuItem = new MenuItem
            {
                Id = 1,
                Name = createDto.Name,
                MenuId = createDto.MenuId,
                Images = new List<MenuItemImage> { new MenuItemImage { ImageUrl = "image-url" } }
            };

            _mockImageStorageService
                .Setup(service => service.UploadImageAsync(It.IsAny<Stream>()))
                .ReturnsAsync("image-url");

            _mockMenuItemRepository
                .Setup(repo => repo.AddAsync(It.IsAny<MenuItem>()))
                .Returns(Task.CompletedTask)
                .Callback<MenuItem>(m => savedMenuItem = m); // Persist mock object

            _mockMapper
                .Setup(m => m.Map<MenuItemDto>(It.IsAny<MenuItem>()))
                .Returns(new MenuItemDto { Id = 1, Name = "Sushi" });

            _mockMenuItemRepository
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => id == savedMenuItem.Id ? savedMenuItem : null);

            var result = await _menuItemService.CreateMenuItemAsync(createDto, images);

            Assert.NotNull(result);
            Assert.Equal("Sushi", result.Name);
        }


        [Fact]
        public async Task UpdateMenuItemAsync_ShouldUpdateMenuItem_WhenIdIsValid()
        {
            var updateDto = new UpdateMenuItemDto { Name = "Pasta", Price = 12.99M };
            var existingItem = new MenuItem { Id = 1, Name = "Old Pasta", Price = 10.99M };

            _mockMenuItemRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingItem);
            _mockMenuItemRepository.Setup(repo => repo.UpdateAsync(existingItem)).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<MenuItemDto>(existingItem)).Returns(new MenuItemDto { Id = 1, Name = "Pasta", Price = 12.99M });

            var result = await _menuItemService.UpdateMenuItemAsync(1, updateDto, null);

            Assert.NotNull(result);
            Assert.Equal("Pasta", result.Name);
            Assert.Equal(12.99M, result.Price);
        }

        [Fact]
        public async Task DeleteMenuItemAsync_ShouldDeleteMenuItem_WhenIdIsValid()
        {
            var menuItem = new MenuItem { Id = 1, Name = "Steak", Images = new List<MenuItemImage> { new MenuItemImage { ImageUrl = "image-url" } } };

            _mockMenuItemRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(menuItem);
            _mockImageStorageService.Setup(service => service.DeleteImageAsync("image-url")).ReturnsAsync(true);
            _mockMenuItemRepository.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

            var result = await _menuItemService.DeleteMenuItemAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteMenuItemImageAsync_ShouldDeleteImage_WhenImageExists()
        {
            var menuItem = new MenuItem
            {
                Id = 1,
                Images = new List<MenuItemImage> { new MenuItemImage { Id = 2, ImageUrl = "image-url" } }
            };

            _mockMenuItemRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(menuItem);
            _mockMenuItemRepository.Setup(repo => repo.RemoveImage(It.IsAny<MenuItemImage>())).Verifiable();
            _mockImageStorageService.Setup(service => service.DeleteImageAsync("image-url")).ReturnsAsync(true);
            _mockMenuItemRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _menuItemService.DeleteMenuItemImageAsync(1, 2);

            Assert.True(result.Success);
        }
    }
}
