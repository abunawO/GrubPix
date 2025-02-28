using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using GrubPix.Application.Services;
using GrubPix.Domain.Entities;
using GrubPix.Application.DTO;
using AutoMapper;
using GrubPix.Domain.Interfaces.Repositories;
using GrubPix.Application.Exceptions;

namespace GrubPix.Tests.Services
{
    public class MenuServiceTests
    {
        private readonly Mock<IMenuRepository> _mockMenuRepository;
        private readonly Mock<IMenuItemRepository> _mockMenuItemRepository;
        private readonly Mock<ILogger<MenuService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly MenuService _menuService;

        public MenuServiceTests()
        {
            _mockMenuRepository = new Mock<IMenuRepository>();
            _mockMenuItemRepository = new Mock<IMenuItemRepository>();
            _mockLogger = new Mock<ILogger<MenuService>>();
            _mockMapper = new Mock<IMapper>();

            _menuService = new MenuService(
                _mockMenuRepository.Object,
                _mockMenuItemRepository.Object,
                _mockLogger.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task GetMenusAsync_ShouldReturnListOfMenus()
        {
            var menus = new List<Menu> { new Menu { Id = 1, Name = "Test Menu" } };
            _mockMenuRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(menus);
            _mockMapper.Setup(m => m.Map<IEnumerable<MenuDto>>(menus)).Returns(new List<MenuDto> { new MenuDto { Id = 1, Name = "Test Menu" } });

            var result = await _menuService.GetMenusAsync();

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetMenuByIdAsync_ShouldReturnMenu_WhenIdIsValid()
        {
            var menu = new Menu { Id = 1, Name = "Test Menu" };
            _mockMenuRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(menu);
            _mockMapper.Setup(m => m.Map<MenuDto>(menu)).Returns(new MenuDto { Id = 1, Name = "Test Menu" });

            var result = await _menuService.GetMenuByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetMenuByIdAsync_ShouldThrowException_WhenIdIsInvalid()
        {
            _mockMenuRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Menu)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _menuService.GetMenuByIdAsync(1));
        }

        [Fact]
        public async Task CreateMenuAsync_ShouldCreateMenu_WhenValidDataIsProvided()
        {
            var menuDto = new CreateMenuDto { Name = "New Menu", RestaurantId = 1 };
            var menu = new Menu { Id = 1, Name = "New Menu", RestaurantId = 1 };

            _mockMenuRepository.Setup(repo => repo.AddAsync(It.IsAny<Menu>())).ReturnsAsync(menu);
            _mockMapper.Setup(m => m.Map<MenuDto>(menu)).Returns(new MenuDto { Id = 1, Name = "New Menu" });

            var result = await _menuService.CreateMenuAsync(menuDto);

            Assert.NotNull(result);
            Assert.Equal("New Menu", result.Name);
        }

        [Fact]
        public async Task UpdateMenuAsync_ShouldUpdateMenu_WhenIdExists()
        {
            var menuDto = new UpdateMenuDto { Name = "Updated Menu" };
            var menu = new Menu { Id = 1, Name = "Old Menu" };

            _mockMenuRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(menu);
            _mockMenuRepository.Setup(repo => repo.UpdateAsync(menu)).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<MenuDto>(menu)).Returns(new MenuDto { Id = 1, Name = "Updated Menu" });

            var result = await _menuService.UpdateMenuAsync(1, menuDto);

            Assert.NotNull(result);
            Assert.Equal("Updated Menu", result.Name);
        }

        [Fact]
        public async Task UpdateMenuAsync_ShouldThrowException_WhenIdDoesNotExist()
        {
            _mockMenuRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Menu)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _menuService.UpdateMenuAsync(1, new UpdateMenuDto { Name = "Invalid Update" }));
        }

        [Fact]
        public async Task DeleteMenuAsync_ShouldDeleteMenu_WhenIdExists()
        {
            var menu = new Menu { Id = 1, Name = "Menu to Delete", MenuItems = new List<MenuItem> { new MenuItem { Id = 1 } } };
            _mockMenuRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(menu);
            _mockMenuItemRepository.Setup(repo => repo.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            _mockMenuRepository.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

            var result = await _menuService.DeleteMenuAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteMenuAsync_ShouldThrowException_WhenIdDoesNotExist()
        {
            _mockMenuRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Menu)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _menuService.DeleteMenuAsync(1));
        }
    }
}