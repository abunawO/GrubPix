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

        public MenuController(IMediator mediator)
        {
            _mediator = mediator;
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
            var command = new CreateMenuCommand(menuDto);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetMenuById), new { id = result.Id }, result);
        }

        // Get Menu by ID
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetMenuById(int id)
        {
            var query = new GetMenuByIdQuery(id);
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // Update Menu
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenu(int id, [FromBody] CreateMenuDto menuDto)
        {
            var command = new UpdateMenuCommand(id, menuDto);
            var result = await _mediator.Send(command);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // Delete Menu
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var command = new DeleteMenuCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
