using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GrubPix.Application.DTO;
using GrubPix.Application.Interfaces.Services;
using System.Threading.Tasks;
using GrubPix.Application.Services.Interfaces;
using MediatR;
using GrubPix.Application.Features.User;

namespace GrubPix.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var result = await _mediator.Send(new UpdateUserCommand(id, dto));
            return result ? Ok("User updated successfully.") : BadRequest("User update failed.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _mediator.Send(new DeleteUserCommand(id));
            return result ? NoContent() : NotFound("User not found.");
        }
    }
}
