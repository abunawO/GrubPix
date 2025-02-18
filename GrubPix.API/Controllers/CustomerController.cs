using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediatR;
using GrubPix.Application.DTOs;
using GrubPix.Application.Features.Customer;
using System.Threading.Tasks;
using GrubPix.Application.DTO;

namespace GrubPix.API.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets customer profile.
        /// </summary>
        [HttpGet("profile")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var query = new GetCustomerProfileQuery(int.Parse(userId));
            var result = await _mediator.Send(query);

            return result != null ? Ok(result) : NotFound("Customer profile not found.");
        }

        /// <summary>
        /// Updates customer profile.
        /// </summary>
        [HttpPut("profile")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateCustomerDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var command = new UpdateCustomerCommand(int.Parse(userId), dto);
            var result = await _mediator.Send(command);

            return result ? Ok("Profile updated successfully.") : BadRequest("Profile update failed.");
        }

        /// <summary>
        /// Gets favorite menu items of the customer.
        /// </summary>
        [HttpGet("favorites")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var query = new GetCustomerFavoritesQuery(int.Parse(userId));
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Adds a menu item to favorites.
        /// </summary>
        [HttpPost("favorites/{menuItemId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddToFavorites(int menuItemId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var command = new AddToFavoritesCommand(int.Parse(userId), menuItemId);
            var result = await _mediator.Send(command);

            return result ? Ok("Added to favorites.") : BadRequest("Failed to add.");
        }

        /// <summary>
        /// Removes a menu item from favorites.
        /// </summary>
        [HttpDelete("favorites/{menuItemId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> RemoveFromFavorites(int menuItemId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var command = new RemoveFromFavoritesCommand(int.Parse(userId), menuItemId);
            var result = await _mediator.Send(command);

            return result ? Ok("Removed from favorites.") : BadRequest("Failed to remove.");
        }
    }
}
