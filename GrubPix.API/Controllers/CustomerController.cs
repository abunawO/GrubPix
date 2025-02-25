using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using GrubPix.Application.DTO;
using GrubPix.Application.Features.Customer;
using GrubPix.Application.Common;

namespace GrubPix.API.Controllers
{
    [Authorize(Roles = "Customer")]
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(IMediator mediator, ILogger<CustomerController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Gets customer profile.
        /// </summary>
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<object>.FailResponse("Unauthorized access."));

            _logger.LogInformation("Fetching profile for Customer ID: {UserId}", userId);

            var query = new GetCustomerProfileQuery(int.Parse(userId));
            var result = await _mediator.Send(query);

            if (result == null)
            {
                _logger.LogWarning("Customer profile not found for ID: {UserId}", userId);
                return NotFound(ApiResponse<object>.FailResponse("Customer profile not found."));
            }

            return Ok(ApiResponse<CustomerDto>.SuccessResponse(result, "Customer profile retrieved successfully."));
        }

        /// <summary>
        /// Updates customer profile.
        /// </summary>
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateCustomerDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<object>.FailResponse("Unauthorized access."));

            _logger.LogInformation("Update request received for Customer ID: {UserId}", userId);

            var command = new UpdateCustomerCommand(int.Parse(userId), dto);
            var result = await _mediator.Send(command);

            if (!result)
            {
                _logger.LogWarning("Failed to update profile for Customer ID: {UserId}", userId);
                return BadRequest(ApiResponse<object>.FailResponse("Profile update failed."));
            }

            _logger.LogInformation("Profile updated successfully for Customer ID: {UserId}", userId);
            return Ok(ApiResponse<string>.SuccessResponse("Profile updated successfully."));
        }

        /// <summary>
        /// Gets favorite menu items of the customer.
        /// </summary>
        [HttpGet("favorites")]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<object>.FailResponse("Unauthorized access."));

            _logger.LogInformation("Fetching favorite items for Customer ID: {UserId}", userId);

            var query = new GetCustomerFavoritesQuery(int.Parse(userId));
            var result = await _mediator.Send(query);

            return Ok(ApiResponse<object>.SuccessResponse(result, "Favorites retrieved successfully."));
        }

        /// <summary>
        /// Adds a menu item to favorites.
        /// </summary>
        [HttpPost("favorites/{menuItemId}")]
        public async Task<IActionResult> AddToFavorites(int menuItemId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<object>.FailResponse("Unauthorized access."));

            _logger.LogInformation("Add to favorites request for Customer ID: {UserId}, MenuItem ID: {MenuItemId}", userId, menuItemId);

            var command = new AddToFavoritesCommand(int.Parse(userId), menuItemId);
            var result = await _mediator.Send(command);

            if (!result)
            {
                _logger.LogWarning("Failed to add to favorites for Customer ID: {UserId}, MenuItem ID: {MenuItemId}", userId, menuItemId);
                return BadRequest(ApiResponse<object>.FailResponse("Failed to add."));
            }

            _logger.LogInformation("Added to favorites for Customer ID: {UserId}, MenuItem ID: {MenuItemId}", userId, menuItemId);
            return Ok(ApiResponse<string>.SuccessResponse("Added to favorites."));
        }

        /// <summary>
        /// Removes a menu item from favorites.
        /// </summary>
        [HttpDelete("favorites/{menuItemId}")]
        public async Task<IActionResult> RemoveFromFavorites(int menuItemId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<object>.FailResponse("Unauthorized access."));

            _logger.LogInformation("Remove from favorites request for Customer ID: {UserId}, MenuItem ID: {MenuItemId}", userId, menuItemId);

            var command = new RemoveFromFavoritesCommand(int.Parse(userId), menuItemId);
            var result = await _mediator.Send(command);

            if (!result)
            {
                _logger.LogWarning("Failed to remove from favorites for Customer ID: {UserId}, MenuItem ID: {MenuItemId}", userId, menuItemId);
                return BadRequest(ApiResponse<object>.FailResponse("Failed to remove."));
            }

            _logger.LogInformation("Removed from favorites for Customer ID: {UserId}, MenuItem ID: {MenuItemId}", userId, menuItemId);
            return Ok(ApiResponse<string>.SuccessResponse("Removed from favorites."));
        }
    }
}
