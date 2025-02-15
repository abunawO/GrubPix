using Microsoft.AspNetCore.Mvc;
using MediatR;
using GrubPix.Application.Features.Restaurant;
using GrubPix.Application.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

        // Get All Restaurants
        [AllowAnonymous]
        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetAllRestaurants(
            [FromQuery] string? name,
            [FromQuery] string? sortBy,
            [FromQuery] bool descending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetRestaurantsQuery(name, sortBy, descending, page, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Get Restaurant by ID
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetRestaurantById(int id)
        {
            _logger.LogInformation("Fetching restaurant with ID {RestaurantId}", id);

            var query = new GetRestaurantByIdQuery(id);
            var result = await _mediator.Send(query);
            if (result == null)
            {
                _logger.LogWarning("Restaurant with ID {RestaurantId} not found", id);
                return NotFound();
            }

            return Ok(result);
        }

        // Create Restaurant
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpPost]
        public async Task<IActionResult> CreateRestaurant([FromForm] CreateRestaurantDto restaurantDto, IFormFile imageFile)
        {
            _logger.LogInformation("Creating restaurant: {RestaurantName}", restaurantDto.Name);

            var command = new CreateRestaurantCommand(restaurantDto, imageFile);
            var result = await _mediator.Send(command);

            _logger.LogInformation("Restaurant {RestaurantName} created successfully with ID {RestaurantId}", restaurantDto.Name, result.Id);
            return CreatedAtAction(nameof(GetRestaurantById), new { id = result.Id }, result);
        }


        // Update Restaurant
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRestaurant(int id, [FromForm] CreateRestaurantDto restaurantDto, IFormFile imageFile)
        {
            _logger.LogWarning("Updating restaurant with ID {RestaurantId}", id);

            var command = new UpdateRestaurantCommand(id, restaurantDto, imageFile);
            var result = await _mediator.Send(command);

            if (result == null)
            {
                _logger.LogError("Failed to Update restaurant with ID {RestaurantId}", id);
                return NotFound();
            }

            return Ok(result);
        }

        // Delete Restaurant
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            _logger.LogWarning("Deleting restaurant with ID {RestaurantId}", id);

            var command = new DeleteRestaurantCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
            {
                _logger.LogError("Failed to delete restaurant with ID {RestaurantId}", id);
                return NotFound();
            }

            _logger.LogInformation("Restaurant {RestaurantId} deleted successfully", id);
            return NoContent();
        }
    }
}
