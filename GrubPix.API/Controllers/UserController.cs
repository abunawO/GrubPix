using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GrubPix.Application.DTO;
using GrubPix.Application.Interfaces.Services;
using System.Threading.Tasks;
using GrubPix.Application.Services.Interfaces;

namespace GrubPix.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var updatedUser = await _userService.UpdateUserAsync(id, dto);
            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (result)
                return NoContent();
            return NotFound();
        }
    }
}
