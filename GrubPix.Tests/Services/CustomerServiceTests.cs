using AutoMapper;
using GrubPix.Application.DTO;
using GrubPix.Application.Interfaces;
using GrubPix.Application.Services;
using GrubPix.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace GrubPix.Tests.Services
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<CustomerService>> _mockLogger;
        private readonly CustomerService _customerService;

        public CustomerServiceTests()
        {
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<CustomerService>>();
            _customerService = new CustomerService(_mockCustomerRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ShouldReturnCustomer_WhenCustomerExists()
        {
            var customer = new Customer { Id = 1, Email = "customer@example.com" };
            var customerDto = new CustomerDto { Email = customer.Email };

            _mockCustomerRepository.Setup(repo => repo.GetCustomerByIdAsync(1)).ReturnsAsync(customer);
            _mockMapper.Setup(m => m.Map<CustomerDto>(customer)).Returns(customerDto);

            var result = await _customerService.GetCustomerByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(customer.Email, result.Email);
        }

        [Fact]
        public async Task GetCustomerByEmailAsync_ShouldReturnCustomer_WhenCustomerExists()
        {
            var customer = new Customer { Id = 2, Email = "customer@example.com" };
            var customerDto = new CustomerDto { Email = customer.Email };

            _mockCustomerRepository.Setup(repo => repo.GetCustomerByEmailAsync("customer@example.com")).ReturnsAsync(customer);
            _mockMapper.Setup(m => m.Map<CustomerDto>(customer)).Returns(customerDto);

            var result = await _customerService.GetCustomerByEmailAsync("customer@example.com");

            Assert.NotNull(result);
            Assert.Equal(customer.Email, result.Email);
        }

        [Fact]
        public async Task CreateCustomerAsync_ShouldCreateCustomer_WhenEmailIsNotTaken()
        {
            var customerDto = new BaseUserDto { Email = "new@example.com", Username = "newuser" };
            var newCustomer = new Customer { Id = 3, Email = customerDto.Email };
            var createdCustomerDto = new CustomerDto { Email = newCustomer.Email };

            _mockCustomerRepository.Setup(repo => repo.GetCustomerByEmailAsync(customerDto.Email)).ReturnsAsync((Customer)null);
            _mockMapper.Setup(m => m.Map<Customer>(customerDto)).Returns(newCustomer);
            _mockCustomerRepository.Setup(repo => repo.CreateCustomerAsync(newCustomer)).ReturnsAsync(newCustomer);
            _mockMapper.Setup(m => m.Map<CustomerDto>(newCustomer)).Returns(createdCustomerDto);

            var result = await _customerService.CreateCustomerAsync(customerDto);

            Assert.NotNull(result);
            Assert.Equal(customerDto.Email, result.Email);
        }

        [Fact]
        public async Task CreateCustomerAsync_ShouldThrowException_WhenEmailAlreadyExists()
        {
            var customerDto = new BaseUserDto { Email = "existing@example.com" };
            var existingCustomer = new Customer { Email = customerDto.Email };

            _mockCustomerRepository.Setup(repo => repo.GetCustomerByEmailAsync(customerDto.Email)).ReturnsAsync(existingCustomer);

            await Assert.ThrowsAsync<Exception>(() => _customerService.CreateCustomerAsync(customerDto));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateCustomer_WhenValidDataProvided()
        {
            var updateDto = new UpdateCustomerDto { Email = "updated@example.com" };
            var existingCustomer = new Customer { Id = 1, Email = "old@example.com" };

            _mockCustomerRepository.Setup(repo => repo.GetCustomerByIdAsync(1)).ReturnsAsync(existingCustomer);
            _mockCustomerRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Customer>())).ReturnsAsync(existingCustomer);

            var result = await _customerService.UpdateAsync(1, updateDto);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenCustomerDoesNotExist()
        {
            _mockCustomerRepository.Setup(repo => repo.GetCustomerByIdAsync(99)).ReturnsAsync((Customer)null);

            var result = await _customerService.DeleteAsync(99);

            Assert.False(result);
        }

        [Fact]
        public async Task AddFavoriteAsync_ShouldReturnTrue_WhenAddingValidMenuItem()
        {
            _mockCustomerRepository.Setup(repo => repo.AddFavoriteAsync(1, 10)).Returns(Task.CompletedTask);

            var result = await _customerService.AddFavoriteAsync(1, 10);

            Assert.True(result);
        }

        [Fact]
        public async Task RemoveFavoriteAsync_ShouldReturnTrue_WhenRemovingValidMenuItem()
        {
            _mockCustomerRepository.Setup(repo => repo.RemoveFavoriteAsync(1, 10)).Returns(Task.CompletedTask);

            var result = await _customerService.RemoveFavoriteAsync(1, 10);

            Assert.True(result);
        }

        [Fact]
        public async Task GetFavoriteMenuItemsAsync_ShouldReturnFavorites_WhenCustomerHasFavorites()
        {
            var favoriteItems = new List<MenuItem> { new MenuItem { Id = 10, Name = "Pizza" } };
            var favoriteDtos = new List<MenuItemDto> { new MenuItemDto { Id = 10, Name = "Pizza" } };

            _mockCustomerRepository.Setup(repo => repo.GetFavoriteMenuItemsAsync(1)).ReturnsAsync(favoriteItems);
            _mockMapper.Setup(m => m.Map<List<MenuItemDto>>(favoriteItems)).Returns(favoriteDtos);

            var result = await _customerService.GetFavoriteMenuItemsAsync(1);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Pizza", result.First().Name);
        }
    }
}