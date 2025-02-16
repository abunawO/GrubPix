using Microsoft.AspNetCore.Mvc;
using MediatR;
using GrubPix.Application.DTO;
using GrubPix.Application.Features.Menu;
using Microsoft.AspNetCore.Authorization;

namespace GrubPix.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/menus")]
    public class MenuController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MenuController> _logger;

        public MenuController(IMediator mediator, ILogger<MenuController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // Get All Menus
        [AllowAnonymous]
        [HttpGet]
        [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetAllMenus()
        {
            var query = new GetMenuQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Create Menu
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpPost]
        public async Task<IActionResult> CreateMenu([FromBody] CreateMenuDto menuDto)
        {
            _logger.LogInformation("Creating menu: {MenuName} for Restaurant ID {RestaurantId}", menuDto.Name, menuDto.RestaurantId);

            var command = new CreateMenuCommand(menuDto);
            var result = await _mediator.Send(command);
            _logger.LogInformation("Menu {MenuName} created successfully with ID {MenuId}", menuDto.Name, result.Id);
            return CreatedAtAction(nameof(GetMenuById), new { id = result.Id }, result);
        }

        // Get Menu by ID
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetMenuById(int id)
        {
            _logger.LogInformation("Fetching menu with ID {MenuId}", id);
            var query = new GetMenuByIdQuery(id);
            var result = await _mediator.Send(query);
            if (result == null)
            {
                _logger.LogWarning("Menu with ID {MenuId} not found", id);
                return NotFound();
            }

            return Ok(result);
        }

        // Update Menu
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenu(int id, [FromBody] UpdateMenuDto menuDto)
        {
            _logger.LogWarning("Updating menu with ID {MenuId}", id);
            var command = new UpdateMenuCommand(id, menuDto);
            var result = await _mediator.Send(command);

            if (result == null)
            {
                _logger.LogError("Failed to Update menu with ID {MenuId}", id);
                return NotFound();
            }

            return Ok(result);
        }

        // Delete Menu
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            _logger.LogWarning("Deleting menu with ID {MenuId}", id);
            var command = new DeleteMenuCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
            {
                _logger.LogError("Failed to delete menu with ID {MenuId}", id);
                return NotFound();
            }

            _logger.LogInformation("Menu {MenuId} deleted successfully", id);
            return NoContent();
        }
    }
}
