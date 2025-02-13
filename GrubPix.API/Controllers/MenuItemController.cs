using Microsoft.AspNetCore.Mvc;
using MediatR;
using GrubPix.Application.DTO;
using GrubPix.Application.Features.MenuItem;

namespace GrubPix.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuItemController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MenuItemController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/MenuItem
        [HttpGet]
        [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetAllMenuItems()
        {
            var query = new GetAllMenuItemsQuery();
            var menuItems = await _mediator.Send(query);
            return Ok(menuItems);
        }

        // GET: api/MenuItem/{id}
        [HttpGet("{id}")]
        [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetMenuItemById(int id)
        {
            var query = new GetMenuItemByIdQuery(id);
            var menuItem = await _mediator.Send(query);

            if (menuItem == null)
                return NotFound();

            return Ok(menuItem);
        }

        // POST: api/MenuItem
        [HttpPost]
        public async Task<IActionResult> CreateMenuItem([FromForm] CreateMenuItemDto menuItemDto, IFormFile imageFile)
        {
            var command = new CreateMenuItemCommand(menuItemDto, imageFile);
            var createdMenuItem = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetMenuItemById), new { id = createdMenuItem.Id }, createdMenuItem);
        }

        // PUT: api/MenuItem/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromForm] CreateMenuItemDto menuItemDto, IFormFile imageFile)
        {
            var command = new UpdateMenuItemCommand(id, menuItemDto, imageFile);
            var updatedMenuItem = await _mediator.Send(command);

            if (updatedMenuItem == null)
                return NotFound();

            return Ok(updatedMenuItem);
        }

        // DELETE: api/MenuItem/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var command = new DeleteMenuItemCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
