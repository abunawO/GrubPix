using Microsoft.AspNetCore.Mvc;
using MediatR;
using GrubPix.Application.DTO;
using GrubPix.Application.Features.MenuItem;
using Microsoft.AspNetCore.Authorization;

namespace GrubPix.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MenuItemController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MenuItemController> _logger;

        public MenuItemController(IMediator mediator, ILogger<MenuItemController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // GET: api/MenuItem
        [AllowAnonymous]
        [HttpGet]
        [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetAllMenuItems()
        {
            var query = new GetAllMenuItemsQuery();
            var menuItems = await _mediator.Send(query);
            return Ok(menuItems);
        }

        // GET: api/MenuItem/{id}
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetMenuItemById(int id)
        {
            _logger.LogInformation("Fetching menu item with ID {ItemId}", id);
            var query = new GetMenuItemByIdQuery(id);
            var menuItem = await _mediator.Send(query);

            if (menuItem == null)
            {
                _logger.LogWarning("Menu item with ID {ItemId} not found", id);
                return NotFound();
            }

            return Ok(menuItem);
        }

        // POST: api/MenuItem
        [HttpPost]
        public async Task<IActionResult> CreateMenuItem([FromForm] CreateMenuItemDto menuItemDto, IFormFile imageFile)
        {
            _logger.LogInformation("Creating menu item: {ItemName} for Menu ID {MenuId}", menuItemDto.Name, menuItemDto.MenuId);
            var command = new CreateMenuItemCommand(menuItemDto, imageFile);
            var createdMenuItem = await _mediator.Send(command);
            _logger.LogInformation("Menu item {ItemName} created successfully with ID {ItemId}", createdMenuItem.Name, createdMenuItem.Id);
            return CreatedAtAction(nameof(GetMenuItemById), new { id = createdMenuItem.Id }, createdMenuItem);
        }

        // PUT: api/MenuItem/{id}
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromForm] UpdateMenuItemDto menuItemDto, IFormFile imageFile)
        {
            _logger.LogInformation("Updating menu item: {ItemName} for Menu ID {MenuId}", menuItemDto.Name, menuItemDto.MenuId);
            var command = new UpdateMenuItemCommand(id, menuItemDto, imageFile);
            var updatedMenuItem = await _mediator.Send(command);

            if (updatedMenuItem == null)
            {
                _logger.LogError("Failed to Update menu item with ID {ItemId}", id);
                return NotFound();
            }

            _logger.LogInformation("Menu item {ItemId} Updated successfully", id);
            return Ok(updatedMenuItem);
        }

        // DELETE: api/MenuItem/{id}
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            _logger.LogWarning("Deleting menu item with ID {ItemId}", id);
            var command = new DeleteMenuItemCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
            {
                _logger.LogError("Failed to delete menu item with ID {ItemId}", id);
                return NotFound();
            }

            _logger.LogInformation("Menu item {ItemId} deleted successfully", id);
            return NoContent();
        }
    }
}
