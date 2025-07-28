namespace MunchKing.DTOs
{
    public class SalesSummaryDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        public Dictionary<string, decimal> RevenueByDay { get; set; } = new();
        public Dictionary<string, int> OrdersByDay { get; set; } = new();

        public Dictionary<string, decimal> RevenueByWeek { get; set; } = new();
        public Dictionary<string, decimal> RevenueByMonth { get; set; } = new();
    }

}
