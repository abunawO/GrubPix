using AutoMapper;
using BCrypt.Net;
using GrubPix.Application.DTO;
using GrubPix.Application.DTOs;
using GrubPix.Application.Exceptions;
using GrubPix.Application.Interfaces;
using GrubPix.Application.Interfaces.Services;
using GrubPix.Application.Services.Interfaces;
using GrubPix.Domain.Entities;
using GrubPix.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GrubPix.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            ICustomerRepository customerRepository,
            IMapper mapper,
            IJwtService jwtService,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Create a hash password.
        /// </summary>
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verifies a hash password.
        /// </summary>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        public async Task<BaseUserDto?> AuthenticateAsync(LoginDto dto)
        {
            _logger.LogInformation("Authenticating user with email: {Email}", dto.Email);

            // Try finding the user (Admin or RestaurantOwner)
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user != null && BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                _logger.LogInformation("User {Email} authenticated successfully", dto.Email);

                var token = _jwtService.GenerateToken(user.Id, user.Username, user.Role);
                var userDto = _mapper.Map<UserDto>(user);
                userDto.Token = token;
                return userDto;
            }

            // Try finding the customer if the user is not found
            var customer = await _customerRepository.GetCustomerByEmailAsync(dto.Email);
            if (customer != null && BCrypt.Net.BCrypt.Verify(dto.Password, customer.PasswordHash))
            {
                _logger.LogInformation("Customer {Email} authenticated successfully", dto.Email);

                var token = _jwtService.GenerateToken(customer.Id, customer.Email, "Customer");
                var customerDto = _mapper.Map<CustomerDto>(customer);
                customerDto.Token = token;
                return customerDto;
            }

            // If no user or customer is found, return unauthorized
            _logger.LogWarning("Invalid credentials for email: {Email}", dto.Email);
            return null;
        }


        /// <summary>
        /// Registers a new user.
        /// </summary>
        public async Task<BaseUserDto> RegisterAsync(RegisterDto dto)
        {
            _logger.LogInformation("Registering new user with email: {Email}", dto.Email);

            if (await _userRepository.GetByEmailAsync(dto.Email) != null)
            {
                _logger.LogWarning("Attempted to register duplicate email: {Email}", dto.Email);
                throw new Exception("Email already in use");
            }

            if (dto.Role == "Customer")
            {
                // Creating a Customer instead of a User
                var customer = new Customer
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    Role = dto.Role,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                };

                var createdCustomer = await _customerRepository.CreateCustomerAsync(customer);
                _logger.LogInformation("Customer {Email} registered successfully", dto.Email);
                return _mapper.Map<CustomerDto>(createdCustomer); // Adjust DTO mapping for customers
            }
            else
            {
                // Creating an Admin or RestaurantOwner
                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Role = dto.Role // "Admin" or "RestaurantOwner"
                };

                var createdUser = await _userRepository.AddAsync(user);
                _logger.LogInformation("User {Email} registered successfully", dto.Email);
                return _mapper.Map<UserDto>(createdUser);
            }
        }


        /// <summary>
        /// Updates an existing user.
        /// </summary>
        public async Task<BaseUserDto> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            _logger.LogInformation("Updating user with email: {Email}", dto.Email);

            // Check if the ID belongs to a User (Admin/RestaurantOwner)
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser != null)
            {
                _mapper.Map(dto, existingUser);
                var updatedUser = await _userRepository.UpdateAsync(existingUser);

                _logger.LogInformation("User {Email} updated successfully", dto.Email);
                return _mapper.Map<UserDto>(updatedUser);
            }

            // Check if the ID belongs to a Customer
            var existingCustomer = await _customerRepository.GetCustomerByIdAsync(id);
            if (existingCustomer != null)
            {
                _mapper.Map(dto, existingCustomer);
                var updatedCustomer = await _customerRepository.UpdateAsync(existingCustomer);

                _logger.LogInformation("Customer {Email} updated successfully", dto.Email);
                return _mapper.Map<CustomerDto>(updatedCustomer);
            }

            _logger.LogError("User or Customer with ID {Id} not found.", id);
            throw new NotFoundException($"User or Customer with ID {id} not found.");
        }


        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        public async Task<bool> DeleteUserAsync(int id)
        {
            // Check if the ID belongs to a User (Admin or RestaurantOwner)
            var userExists = await _userRepository.GetByIdAsync(id);
            if (userExists != null)
            {
                var result = await _userRepository.DeleteAsync(id);
                if (result)
                {
                    _logger.LogInformation("User with ID {Id} deleted successfully", id);
                }
                else
                {
                    _logger.LogWarning("Failed to delete user with ID {Id}", id);
                }
                return result;
            }

            // Check if the ID belongs to a Customer
            var customerExists = await _customerRepository.GetCustomerByIdAsync(id);
            if (customerExists != null)
            {
                var result = await _customerRepository.DeleteAsync(id);
                if (result)
                {
                    _logger.LogInformation("Customer with ID {Id} deleted successfully", id);
                }
                else
                {
                    _logger.LogWarning("Failed to delete customer with ID {Id}", id);
                }
                return result;
            }

            _logger.LogWarning("Attempted to delete non-existent user/customer ID: {Id}", id);
            throw new NotFoundException($"User or Customer with ID {id} not found.");
        }

        /// <summary>
        /// Retrieves user or customer details by username.
        /// </summary>
        public async Task<BaseUserDto?> GetByUsernameAsync(string username)
        {
            // Check if the username exists in Users
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user != null) return _mapper.Map<UserDto>(user);

            // Check if the username exists in Customers
            var customer = await _customerRepository.GetCustomerByUsernameAsync(username);
            if (customer != null) return _mapper.Map<CustomerDto>(customer);

            return null; // No match found
        }

        /// <summary>
        /// Retrieves user details by email.
        /// </summary>
        public async Task<BaseUserDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        /// <summary>
        /// Retrieves user details by ID.
        /// </summary>
        public async Task<BaseUserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }
    }
}
