using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GrubPix.Application.DTO;
using GrubPix.Application.Features.User;
using MediatR;
using System.Threading.Tasks;
using GrubPix.Application.Common;

namespace GrubPix.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger;

        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Updates a user by ID.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            try
            {
                _logger.LogInformation("Update request received for User ID {UserId}", id);
                var result = await _mediator.Send(new UpdateUserCommand(id, dto));

                if (result)
                {
                    _logger.LogInformation("User {UserId} updated successfully", id);
                    return Ok(ApiResponse<string>.SuccessResponse("User updated successfully."));
                }

                _logger.LogWarning("Update failed for User ID {UserId}", id);
                return BadRequest(ApiResponse<object>.FailResponse("User update failed."));
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("User {UserId} not found for update", id);
                return NotFound(ApiResponse<object>.FailResponse("User not found."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", id);
                return StatusCode(500, ApiResponse<object>.FailResponse(ex.Message));
            }
        }

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                _logger.LogInformation("Delete request received for User ID {UserId}", id);
                var result = await _mediator.Send(new DeleteUserCommand(id));

                if (result)
                {
                    _logger.LogInformation("User {UserId} deleted successfully", id);
                    return Ok(ApiResponse<string>.SuccessResponse("User deleted successfully."));
                }

                _logger.LogWarning("User {UserId} not found for deletion", id);
                return NotFound(ApiResponse<object>.FailResponse("User not found."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                return StatusCode(500, ApiResponse<object>.FailResponse(ex.Message));
            }
        }
    }
}
