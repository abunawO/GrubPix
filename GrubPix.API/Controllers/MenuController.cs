using Microsoft.AspNetCore.Mvc;
using MediatR;
using GrubPix.Application.DTO;
using GrubPix.Application.Features.Menu;
using Microsoft.AspNetCore.Authorization;
using GrubPix.Application.Common;

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
        // GET: api/Menu
        [AllowAnonymous]
        [HttpGet]
        [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetAllMenus()
        {
            try
            {
                _logger.LogInformation("Fetching all menus...");
                var query = new GetMenuQuery();
                var result = await _mediator.Send(query);
                return Ok(ApiResponse<object>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching menus.");
                return StatusCode(500, ApiResponse<object>.FailResponse(ex.Message));
            }
        }

        // Create Menu
        // POST: api/Menu
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpPost]
        public async Task<IActionResult> CreateMenu([FromBody] CreateMenuDto menuDto)
        {
            try
            {
                _logger.LogInformation("Creating menu: {MenuName} for Restaurant ID {RestaurantId}", menuDto.Name, menuDto.RestaurantId);
                var command = new CreateMenuCommand(menuDto);
                var result = await _mediator.Send(command);
                _logger.LogInformation("Menu {MenuName} created successfully with ID {MenuId}", menuDto.Name, result.Id);
                return CreatedAtAction(nameof(GetMenuById), new { id = result.Id }, ApiResponse<object>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the menu.");
                return StatusCode(500, ApiResponse<object>.FailResponse(ex.Message));
            }
        }

        // Get Menu by ID
        // GET: api/Menu/{id}
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetMenuById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching menu with ID {MenuId}", id);
                var query = new GetMenuByIdQuery(id);
                var result = await _mediator.Send(query);
                if (result == null)
                {
                    _logger.LogWarning("Menu with ID {MenuId} not found", id);
                    return NotFound(ApiResponse<object>.FailResponse("Menu not found"));
                }
                return Ok(ApiResponse<object>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching menu with ID {MenuId}", id);
                return StatusCode(500, ApiResponse<object>.FailResponse(ex.Message));
            }
        }

        // Update Menu
        // PUT: api/Menu/{id}
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenu(int id, [FromBody] UpdateMenuDto menuDto)
        {
            try
            {
                _logger.LogInformation("Updating menu with ID {MenuId}", id);
                var command = new UpdateMenuCommand(id, menuDto);
                var result = await _mediator.Send(command);

                if (result == null)
                {
                    _logger.LogError("Failed to update menu with ID {MenuId}", id);
                    return NotFound(ApiResponse<object>.FailResponse("Menu not found"));
                }

                _logger.LogInformation("Menu {MenuId} updated successfully", id);
                return Ok(ApiResponse<object>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating menu with ID {MenuId}", id);
                return StatusCode(500, ApiResponse<object>.FailResponse(ex.Message));
            }
        }

        // Delete Menu
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            try
            {
                _logger.LogWarning("Deleting menu with ID {MenuId}", id);
                var command = new DeleteMenuCommand(id);
                var result = await _mediator.Send(command);

                if (!result)
                {
                    _logger.LogError("Failed to delete menu with ID {MenuId}", id);
                    return NotFound(ApiResponse<object>.FailResponse("Menu not found"));
                }

                _logger.LogInformation("Menu {MenuId} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting menu with ID {MenuId}", id);
                return StatusCode(500, ApiResponse<object>.FailResponse(ex.Message));
            }
        }
    }
}
