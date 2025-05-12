using MunchKing.DTOs;
using Microsoft.EntityFrameworkCore;
using MunchKing.Context;
using MunchKing.Enums;

namespace MunchKing.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardStatsDto> GetStatisticsAsync()
        {
            var stats = new AdminDashboardStatsDto
            {
                TotalOrders = await _context.Orders.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(),
                TotalFoodItems = await _context.FoodItems.CountAsync(),
                TotalRevenue = await _context.Orders
                    .Where(o => o.Status == Enums.OrderStatus.Completed)
                    .SelectMany(o => o.OrderItems)
                    .SumAsync(oi => oi.UnitPrice * oi.Quantity),
                OrdersPerStatus = await _context.Orders
                    .GroupBy(o => o.Status.ToString())
                    .ToDictionaryAsync(g => g.Key, g => g.Count()),
                TopSellingItems = await _context.OrderItems
                    .Include(oi => oi.FoodItem)
                    .GroupBy(oi => oi.FoodItem.Name)
                    .OrderByDescending(g => g.Sum(x => x.Quantity))
                    .Take(5)
                    .Select(g => new TopSellingItemDto
                    {
                        FoodName = g.Key,
                        QuantitySold = g.Sum(x => x.Quantity)
                    }).ToListAsync()
            };

            return stats;
        }
    }
}
