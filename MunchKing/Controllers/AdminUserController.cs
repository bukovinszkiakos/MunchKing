using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MunchKing.Services;

namespace MunchKing.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminUserController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminUserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPut("{userId}/toggle-admin")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ToggleAdminRole(string userId)
        {
            var result = await _userService.ToggleAdminRoleAsync(userId);
            return result ? Ok() : BadRequest("Could not update role");
        }

    }



}
