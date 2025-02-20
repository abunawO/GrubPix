using Microsoft.AspNetCore.Mvc;
using MediatR;
using GrubPix.Application.Features.Restaurant;
using GrubPix.Application.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GrubPix.Application.Common;

namespace GrubPix.API.Controllers
{
    [Authorize] // <-- Secure all endpoints in this controller 
    [ApiController]
    [Route("api/restaurants")]
    public class RestaurantController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public RestaurantController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // Get All Restaurants associated to a user
        [AllowAnonymous]
        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetRestaurants(
            [FromQuery] string? name,
            [FromQuery] string? sortBy,
            [FromQuery] bool descending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                int? userId = string.IsNullOrEmpty(userIdClaim) ? null : int.Parse(userIdClaim);
                string role = roleClaim ?? "";

                _logger.LogInformation("Fetching restaurants with user ID {UserId} and role {Role}", userId, role);

                var query = new GetRestaurantsQuery(name, sortBy, descending, page, pageSize, userId, role);
                var result = await _mediator.Send(query);

                return Ok(ApiResponse<List<RestaurantDto>>.SuccessResponse(result, "Restaurants fetched successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching restaurants");
                return StatusCode(500, ApiResponse<object>.FailResponse(ex.Message));
            }
        }


        // Get Restaurant by ID
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetRestaurantById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching restaurant with ID {RestaurantId}", id);

                var query = new GetRestaurantByIdQuery(id);
                var result = await _mediator.Send(query);

                if (result == null)
                {
                    _logger.LogWarning("Restaurant with ID {RestaurantId} not found", id);
                    return NotFound(ApiResponse<object>.FailResponse("Restaurant not found"));
                }

                return Ok(ApiResponse<RestaurantDto>.SuccessResponse(result, "Restaurant fetched successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching restaurant with ID {RestaurantId}", id);
                return StatusCode(500, ApiResponse<object>.FailResponse(ex.Message));
            }
        }

        // Create Restaurant
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpPost]
        public async Task<IActionResult> CreateRestaurant([FromForm] CreateRestaurantDto restaurantDto, IFormFile imageFile)
        {
            try
            {
                _logger.LogInformation("Creating restaurant: {RestaurantName}", restaurantDto.Name);

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                restaurantDto.OwnerId = userId; // Ensure the restaurant is linked to the owner

                var command = new CreateRestaurantCommand(restaurantDto, imageFile);
                var result = await _mediator.Send(command);

                _logger.LogInformation("Restaurant {RestaurantName} created successfully with ID {RestaurantId}", restaurantDto.Name, result.Id);
                return CreatedAtAction(nameof(GetRestaurantById), new { id = result.Id }, ApiResponse<RestaurantDto>.SuccessResponse(result, "Restaurant created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating restaurant {RestaurantName}", restaurantDto.Name);
                return StatusCode(500, ApiResponse<object>.FailResponse(ex.Message));
            }
        }

        // Update Restaurant
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRestaurant(int id, [FromForm] UpdateRestaurantDto restaurantDto, IFormFile imageFile)
        {
            try
            {
                _logger.LogInformation("Updating restaurant with ID {RestaurantId}", id);

                var command = new UpdateRestaurantCommand(id, restaurantDto, imageFile);
                var result = await _mediator.Send(command);

                if (result == null)
                {
                    _logger.LogError("Failed to update restaurant with ID {RestaurantId}", id);
                    return NotFound(ApiResponse<object>.FailResponse("Restaurant not found"));
                }

                return Ok(ApiResponse<RestaurantDto>.SuccessResponse(result, "Restaurant updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating restaurant with ID {RestaurantId}", id);
                return StatusCode(500, ApiResponse<object>.FailResponse(ex.Message));
            }
        }

        // Delete Restaurant
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            try
            {
                _logger.LogWarning("Deleting restaurant with ID {RestaurantId}", id);

                var command = new DeleteRestaurantCommand(id);
                var result = await _mediator.Send(command);

                if (!result)
                {
                    _logger.LogError("Failed to delete restaurant with ID {RestaurantId}", id);
                    return NotFound(ApiResponse<object>.FailResponse("Restaurant not found"));
                }

                _logger.LogInformation("Restaurant {RestaurantId} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting restaurant with ID {RestaurantId}", id);
                return StatusCode(500, ApiResponse<object>.FailResponse(ex.Message));
            }
        }

    }
}
