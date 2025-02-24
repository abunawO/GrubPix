using Microsoft.AspNetCore.Mvc;
using MediatR;
using GrubPix.Application.DTO;
using GrubPix.Application.Features.MenuItem;
using Microsoft.AspNetCore.Authorization;
using GrubPix.Application.Common;
using System.Net;

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
            try
            {
                _logger.LogInformation("Fetching all menu items.");
                var query = new GetAllMenuItemsQuery();
                var menuItems = await _mediator.Send(query);
                return Ok(ApiResponse<IEnumerable<MenuItemDto>>.SuccessResponse(menuItems, "Menu Items fetched successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching menu items.");
                return StatusCode((int)HttpStatusCode.InternalServerError, ApiResponse<object>.FailResponse(ex.Message));
            }
        }

        // GET: api/MenuItem/{id}
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetMenuItemById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching menu item with ID {ItemId}", id);
                var query = new GetMenuItemByIdQuery(id);
                var menuItem = await _mediator.Send(query);

                if (menuItem == null)
                {
                    _logger.LogWarning("Menu item with ID {ItemId} not found", id);
                    return NotFound(ApiResponse<object>.FailResponse("Menu item not found"));
                }

                return Ok(ApiResponse<MenuItemDto>.SuccessResponse(menuItem));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching menu item with ID {ItemId}", id);
                return StatusCode((int)HttpStatusCode.InternalServerError, ApiResponse<object>.FailResponse(ex.Message));
            }
        }

        // POST: api/MenuItem
        // PUT: api/MenuItem/{id}
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpPost]
        public async Task<IActionResult> CreateMenuItem([FromForm] CreateMenuItemDto menuItemDto, [FromForm] List<IFormFile> imageFiles)
        {
            try
            {
                _logger.LogInformation("Creating menu item: {ItemName} for Menu ID {MenuId}", menuItemDto.Name, menuItemDto.MenuId);
                var command = new CreateMenuItemCommand(menuItemDto, imageFiles);
                var createdMenuItem = await _mediator.Send(command);

                _logger.LogInformation("Menu item {ItemName} created successfully with ID {ItemId}", createdMenuItem.Name, createdMenuItem.Id);
                return CreatedAtAction(nameof(GetMenuItemById), new { id = createdMenuItem.Id }, ApiResponse<MenuItemDto>.SuccessResponse(createdMenuItem));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating menu item: {ItemName}", menuItemDto.Name);
                return StatusCode((int)HttpStatusCode.InternalServerError, ApiResponse<object>.FailResponse(ex.Message));
            }
        }

        // PUT: api/MenuItem/{id}
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromForm] UpdateMenuItemDto menuItemDto, [FromForm] List<IFormFile> imageFiles)
        {
            try
            {
                _logger.LogInformation("Updating menu item: {ItemName} for Menu ID {MenuId}", menuItemDto.Name, menuItemDto.MenuId);
                var command = new UpdateMenuItemCommand(id, menuItemDto, imageFiles);
                var updatedMenuItem = await _mediator.Send(command);

                if (updatedMenuItem == null)
                {
                    _logger.LogError("Failed to update menu item with ID {ItemId}", id);
                    return NotFound(ApiResponse<object>.FailResponse("Menu item not found"));
                }

                _logger.LogInformation("Menu item {ItemId} updated successfully", id);
                return Ok(ApiResponse<MenuItemDto>.SuccessResponse(updatedMenuItem));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating menu item with ID {ItemId}", id);
                return StatusCode((int)HttpStatusCode.InternalServerError, ApiResponse<object>.FailResponse(ex.Message));
            }
        }

        // DELETE: api/MenuItem/{id}
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            try
            {
                _logger.LogWarning("Deleting menu item with ID {ItemId}", id);
                var command = new DeleteMenuItemCommand(id);
                var result = await _mediator.Send(command);

                if (!result)
                {
                    _logger.LogError("Failed to delete menu item with ID {ItemId}", id);
                    return NotFound(ApiResponse<object>.FailResponse("Menu item not found"));
                }

                _logger.LogInformation("Menu item {ItemId} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting menu item with ID {ItemId}", id);
                return StatusCode((int)HttpStatusCode.InternalServerError, ApiResponse<object>.FailResponse(ex.Message));
            }
        }
    }
}
