using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MunchKing.Services.Admin;
using MunchKing.DTOs;

namespace MunchKing.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var success = await _adminService.DeleteUserAsync(userId);
            return success ? Ok(new { message = "User deleted." }) : NotFound(new { message = "User not found." });
        }

    }
}
