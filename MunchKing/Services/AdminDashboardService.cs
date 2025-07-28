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
                var completedOrders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .Where(o => o.Status == OrderStatus.Completed)
                    .ToListAsync();

                var totalRevenue = completedOrders
                    .SelectMany(o => o.OrderItems)
                    .Sum(oi => oi.UnitPrice * oi.Quantity);

                var ordersPerStatus = (await _context.Orders
                    .GroupBy(o => o.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToListAsync())
                    .ToDictionary(g => g.Status.ToString(), g => g.Count);

                var topSellingItems = await _context.OrderItems
                    .Include(oi => oi.FoodItem)
                    .GroupBy(oi => oi.FoodItem.Name)
                    .OrderByDescending(g => g.Sum(x => x.Quantity))
                    .Take(5)
                    .Select(g => new TopSellingItemDto
                    {
                        FoodName = g.Key,
                        QuantitySold = g.Sum(x => x.Quantity)
                    })
                    .ToListAsync();

                var nonAdminUserCount = await (
                    from user in _context.Users
                    join userRole in _context.UserRoles on user.Id equals userRole.UserId
                    join role in _context.Roles on userRole.RoleId equals role.Id
                    group role.Name by user.Id into grouped
                    where !grouped.Any(roleName => roleName == "Admin" || roleName == "SuperAdmin")
                    select grouped.Key
                ).CountAsync();

                var stats = new AdminDashboardStatsDto
                {
                    TotalOrders = await _context.Orders.CountAsync(),
                    TotalUsers = nonAdminUserCount,
                    TotalFoodItems = await _context.FoodItems.CountAsync(),
                    TotalRevenue = totalRevenue,
                    OrdersPerStatus = ordersPerStatus,
                    TopSellingItems = topSellingItems,
                    TotalFeedbacks = await _context.ContactMessages.CountAsync(),
                    TotalCategories = await _context.Categories.CountAsync()
                };

                return stats;
            }

        }
    }
