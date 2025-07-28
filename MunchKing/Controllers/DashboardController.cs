using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MunchKing.Services;

namespace MunchKing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class DashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _dashboardService;

        public DashboardController(IAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _dashboardService.GetStatisticsAsync();
            return Ok(stats);
        }
    }
}
