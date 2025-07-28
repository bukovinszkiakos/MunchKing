using MunchKing.DTOs;

namespace MunchKing.Services
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardStatsDto> GetStatisticsAsync();
    }
}
