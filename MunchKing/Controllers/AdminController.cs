using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MunchKing.Services.Admin;
using MunchKing.DTOs;
using MunchKing.Services;

namespace MunchKing.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IAdminDashboardService _dashboardService;

        public AdminController(IAdminService adminService, IAdminDashboardService dashboardService)
        {
            _adminService = adminService;
            _dashboardService = dashboardService;
        }

        /*[HttpGet("users")]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }
        */

        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var success = await _adminService.DeleteUserAsync(userId);
            return success ? Ok(new { message = "User deleted." }) : NotFound(new { message = "User not found." });
        }

        [HttpGet("dashboard/stats")]
        public async Task<ActionResult<AdminDashboardStatsDto>> GetDashboardStats()
        {
            var stats = await _dashboardService.GetStatisticsAsync();
            return Ok(stats);
        }

    }
}
