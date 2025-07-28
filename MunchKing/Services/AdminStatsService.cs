using MunchKing.Context;
using MunchKing.DTOs;
using Microsoft.EntityFrameworkCore;
using MunchKing.Enums;

public class AdminStatsService : IAdminStatsService
{
    private readonly ApplicationDbContext _db;

    public AdminStatsService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<SalesSummaryDto> GetSalesSummaryAsync()
    {
        var completedOrders = await _db.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.Status == OrderStatus.Completed)
            .ToListAsync();

        var summary = new SalesSummaryDto
        {
            TotalOrders = completedOrders.Count,
            TotalRevenue = completedOrders.Sum(o =>
                o.TotalAmount > 0
                    ? o.TotalAmount
                    : o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity)
            )
        };

        foreach (var order in completedOrders)
        {
            var date = order.CreatedAt.Date.ToString("yyyy-MM-dd");
            var week = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                order.CreatedAt, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday
            );
            var weekKey = $"{order.CreatedAt.Year}-W{week}";
            var monthKey = order.CreatedAt.ToString("yyyy-MM");

            var orderTotal = order.TotalAmount > 0
                ? order.TotalAmount
                : order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity);

            if (!summary.RevenueByDay.ContainsKey(date))
                summary.RevenueByDay[date] = 0;
            summary.RevenueByDay[date] += orderTotal;

            if (!summary.OrdersByDay.ContainsKey(date))
                summary.OrdersByDay[date] = 0;
            summary.OrdersByDay[date] += 1;

            if (!summary.RevenueByWeek.ContainsKey(weekKey))
                summary.RevenueByWeek[weekKey] = 0;
            summary.RevenueByWeek[weekKey] += orderTotal;

            if (!summary.RevenueByMonth.ContainsKey(monthKey))
                summary.RevenueByMonth[monthKey] = 0;
            summary.RevenueByMonth[monthKey] += orderTotal;
        }

        return summary;
    }




    public async Task<List<TopProductDto>> GetTopSellingItemsAsync()
    {
        return await _db.OrderItems
            .Where(oi => oi.Order.Status == OrderStatus.Completed)
            .GroupBy(oi => oi.FoodItem.Name)
            .Select(g => new TopProductDto
            {
                FoodName = g.Key,
                QuantitySold = g.Sum(x => x.Quantity)
            })
            .OrderByDescending(x => x.QuantitySold)
            .Take(5)
            .ToListAsync();
    }
}
