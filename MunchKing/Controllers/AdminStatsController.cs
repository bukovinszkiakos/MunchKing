using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MunchKing.DTOs;
using MunchKing.Services;

namespace MunchKing.Controllers
{
    [ApiController]
    [Route("api/admin/stats")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminStatsController : ControllerBase
    {
        private readonly IAdminStatsService _statsService;

        public AdminStatsController(IAdminStatsService statsService)
        {
            _statsService = statsService;
        }

        [HttpGet("sales-summary")]
        public async Task<ActionResult<SalesSummaryDto>> GetSalesSummary()
        {
            var data = await _statsService.GetSalesSummaryAsync();
            return Ok(data);
        }

        [HttpGet("top-products")]
        public async Task<ActionResult<List<TopProductDto>>> GetTopSellingItems()
        {
            var items = await _statsService.GetTopSellingItemsAsync();
            return Ok(items);
        }
    }
}
