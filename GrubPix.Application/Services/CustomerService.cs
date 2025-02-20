using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GrubPix.Application.DTO;
using GrubPix.Application.DTOs;
using GrubPix.Application.Interfaces;
using GrubPix.Application.Interfaces.Services;
using GrubPix.Domain.Entities;
using GrubPix.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace GrubPix.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper, ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(int id)
        {
            _logger.LogInformation("Fetching customer with ID: {Id}", id);
            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            return customer is not null ? _mapper.Map<CustomerDto>(customer) : null;
        }

        public async Task<CustomerDto> GetCustomerByEmailAsync(string email)
        {
            _logger.LogInformation("Fetching customer with Email: {Email}", email);
            var customer = await _customerRepository.GetCustomerByEmailAsync(email);
            return customer is not null ? _mapper.Map<CustomerDto>(customer) : null;
        }

        public async Task<CustomerDto> GetCustomerByUsernameAsync(string username)
        {
            _logger.LogInformation("Fetching customer with Username: {Username}", username);
            var customer = await _customerRepository.GetCustomerByUsernameAsync(username);
            return customer is not null ? _mapper.Map<CustomerDto>(customer) : null;
        }

        public async Task<CustomerDto> CreateCustomerAsync(BaseUserDto dto)
        {
            _logger.LogInformation("Creating customer with Email: {Email}", dto.Email);

            // Prevent duplicate email registration
            var existingCustomer = await _customerRepository.GetCustomerByEmailAsync(dto.Email);
            if (existingCustomer is not null)
            {
                _logger.LogWarning("Duplicate email registration attempted: {Email}", dto.Email);
                throw new Exception("Email already in use");
            }

            var customer = _mapper.Map<Customer>(dto);
            var createdCustomer = await _customerRepository.CreateCustomerAsync(customer);
            return _mapper.Map<CustomerDto>(createdCustomer);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCustomerDto dto)
        {
            _logger.LogInformation("Updating customer with ID: {Id}", id);

            var existingCustomer = await _customerRepository.GetCustomerByIdAsync(id);
            if (existingCustomer is null)
            {
                _logger.LogError("Customer with ID: {Id} not found.", id);
                return false;
            }

            // Prevent updating to an already existing email (unless it's the same user)
            if (!existingCustomer.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailExists = await _customerRepository.GetCustomerByEmailAsync(dto.Email);
                if (emailExists is not null)
                {
                    _logger.LogWarning("Duplicate email update attempted: {Email}", dto.Email);
                    throw new Exception("Email already in use");
                }
            }

            _mapper.Map(dto, existingCustomer);
            await _customerRepository.UpdateAsync(existingCustomer);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting customer with ID: {Id}", id);

            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            if (customer is null)
            {
                _logger.LogWarning("Attempted to delete non-existent customer with ID: {Id}", id);
                return false;
            }

            return await _customerRepository.DeleteAsync(id);
        }

        public async Task<bool> AddFavoriteAsync(int customerId, int menuItemId)
        {
            _logger.LogInformation("Adding favorite for Customer ID: {CustomerId}, MenuItem ID: {MenuItemId}", customerId, menuItemId);
            await _customerRepository.AddFavoriteAsync(customerId, menuItemId);
            return true;
        }

        public async Task<bool> RemoveFavoriteAsync(int customerId, int menuItemId)
        {
            _logger.LogInformation("Removing favorite for Customer ID: {CustomerId}, MenuItem ID: {MenuItemId}", customerId, menuItemId);
            await _customerRepository.RemoveFavoriteAsync(customerId, menuItemId);
            return true;
        }

        public async Task<List<MenuItemDto>> GetFavoriteMenuItemsAsync(int customerId)
        {
            _logger.LogInformation("Fetching favorite menu items for Customer ID: {CustomerId}", customerId);
            var favorites = await _customerRepository.GetFavoriteMenuItemsAsync(customerId);
            return _mapper.Map<List<MenuItemDto>>(favorites);
        }
    }

}
