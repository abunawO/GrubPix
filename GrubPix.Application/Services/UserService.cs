using AutoMapper;
using BCrypt.Net;
using GrubPix.Application.DTO;
using GrubPix.Application.Exceptions;
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
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            IJwtService jwtService,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
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
        public async Task<UserDto?> AuthenticateAsync(LoginDto dto)
        {
            _logger.LogInformation("Authenticating user with email: {Email}", dto.Email);

            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid credentials for email: {Email}", dto.Email);
                return null;
            }

            // Restrict login to only Admins or RestaurantOwners
            if (user.Role != "Admin" && user.Role != "RestaurantOwner")
            {
                _logger.LogWarning("Unauthorized login attempt by: {Email}", dto.Email);
                return null;
            }

            var token = _jwtService.GenerateToken(user.Id, user.Username, user.Role);
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Token = token;

            _logger.LogInformation("User {Email} authenticated successfully", dto.Email);
            return userDto;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            _logger.LogInformation("Registering new user with email: {Email}", dto.Email);

            if (await _userRepository.GetByEmailAsync(dto.Email) != null)
            {
                _logger.LogWarning("Attempted to register duplicate email: {Email}", dto.Email);
                throw new Exception("Email already in use");
            }

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "RestaurantOwner" // Default role
            };

            var createdUser = await _userRepository.AddAsync(user);

            _logger.LogInformation("User {Email} registered successfully", dto.Email);
            return _mapper.Map<UserDto>(createdUser);
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            _logger.LogInformation("Updating user with email: {Email}", dto.Email);

            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser == null)
            {
                _logger.LogError("User with email {Email} not found.", dto.Email);
                throw new NotFoundException($"User with email {dto.Email} not found.");
            }

            _mapper.Map(dto, existingUser);
            var updatedUser = await _userRepository.UpdateAsync(existingUser);

            _logger.LogInformation("User {Email} updated successfully", dto.Email);
            return _mapper.Map<UserDto>(updatedUser);
        }

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        public async Task<bool> DeleteUserAsync(int id)
        {
            var userExists = await _userRepository.GetByIdAsync(id);
            if (userExists == null)
            {
                _logger.LogWarning("Attempted to delete non-existent user ID: {Id}", id);
                throw new NotFoundException($"User with ID {id} not found.");
            }

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

        /// <summary>
        /// Retrieves user details by username.
        /// </summary>
        public async Task<UserDto?> GetByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        /// <summary>
        /// Retrieves user details by email.
        /// </summary>
        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        /// <summary>
        /// Retrieves user details by ID.
        /// </summary>
        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }
    }
}
