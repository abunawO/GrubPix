using Microsoft.AspNetCore.Mvc;
using MediatR;
using GrubPix.Application.Features.Restaurant;
using GrubPix.Application.DTO;

namespace GrubPix.API.Controllers
{
    [ApiController]
    [Route("api/restaurants")]
    public class RestaurantController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RestaurantController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Get All Restaurants
        [HttpGet]
        public async Task<IActionResult> GetAllRestaurants()
        {
            var query = new GetRestaurantsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Get Restaurant by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRestaurantById(int id)
        {
            var query = new GetRestaurantByIdQuery(id);
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // Create Restaurant
        [HttpPost]
        public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantDto restaurantDto)
        {
            var command = new CreateRestaurantCommand(restaurantDto);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetRestaurantById), new { id = result.Id }, result);
        }

        // Update Restaurant
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRestaurant(int id, [FromBody] CreateRestaurantDto restaurantDto)
        {
            var command = new UpdateRestaurantCommand(id, restaurantDto);
            var result = await _mediator.Send(command);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // Delete Restaurant
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var command = new DeleteRestaurantCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
