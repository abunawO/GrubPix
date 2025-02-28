using AutoMapper;
using BCrypt.Net;
using GrubPix.Application.DTO;
using GrubPix.Application.Exceptions;
using GrubPix.Application.Interfaces;
using GrubPix.Application.Interfaces.Services;
using GrubPix.Application.Services.Interfaces;
using GrubPix.Application.Utils;
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
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                // If email is being changed, ensure it's not already taken
                if (!existingUser.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase) &&
                    await _userRepository.GetByEmailAsync(dto.Email) != null)
                {
                    _logger.LogWarning("Attempted to update duplicate email: {Email}", dto.Email);
                    throw new Exception("Email is already in use by another account.");
                }

                _mapper.Map(dto, existingUser);
                var updatedUser = await _userRepository.UpdateAsync(existingUser);

                _logger.LogInformation("User {Email} updated successfully", dto.Email);
                return _mapper.Map<UserDto>(updatedUser);
            }
            else
            {
                _logger.LogError("User with ID {Id} not found.", id);
                throw new NotFoundException($"User with ID {id} not found.");
            }
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
            else
            {
                _logger.LogWarning("Attempted to delete non-existent user ID: {Id}", id);
                throw new NotFoundException($"User with ID {id} not found.");
            }

        }

        /// <summary>
        /// Retrieves user or customer details by username.
        /// </summary>
        public async Task<BaseUserDto?> GetByUsernameAsync(string username)
        {
            // Check if the username exists in Users
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user != null) return _mapper.Map<UserDto>(user);

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
