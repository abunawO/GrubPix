using Microsoft.AspNetCore.Mvc;
using MediatR;
using GrubPix.Application.DTO;
using GrubPix.Application.Features.Menu;

namespace GrubPix.API.Controllers
{
    [ApiController]
    [Route("api/menus")]
    public class MenuController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MenuController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Create Menu
        [HttpPost]
        public async Task<IActionResult> CreateMenu([FromBody] CreateMenuDto menuDto)
        {
            var command = new CreateMenuCommand { MenuDto = menuDto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetMenuById), new { id = result.Id }, result);
        }

        // Get All Menus
        [HttpGet]
        public async Task<IActionResult> GetAllMenus()
        {
            var query = new GetMenuQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Get Menu by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenuById(int id)
        {
            var query = new GetMenuByIdQuery(id);
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // Update Menu
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenu(int id, [FromBody] CreateMenuDto menuDto)
        {
            var command = new UpdateMenuCommand { Id = id, MenuDto = menuDto };
            var result = await _mediator.Send(command);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // Delete Menu
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
