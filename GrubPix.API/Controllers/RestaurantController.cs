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

        [HttpGet("test-exception")]
        public IActionResult TestException()
        {
            throw new Exception("This is a test exception.");
        }

        // Get All Restaurants
        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetAllRestaurants()
        {
            var query = new GetRestaurantsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Get Restaurant by ID
        [HttpGet("{id}")]
        [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Client, NoStore = false)]
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
        public async Task<IActionResult> CreateRestaurant([FromForm] CreateRestaurantDto restaurantDto, IFormFile imageFile)
        {
            var command = new CreateRestaurantCommand(restaurantDto, imageFile);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetRestaurantById), new { id = result.Id }, result);
        }


        // Update Restaurant
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRestaurant(int id, [FromForm] CreateRestaurantDto restaurantDto, IFormFile imageFile)
        {
            var command = new UpdateRestaurantCommand(id, restaurantDto, imageFile);
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
