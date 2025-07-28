namespace MunchKing.DTOs
{
    public class AdminDashboardStatsDto
    {
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalFoodItems { get; set; }

        public int TotalFeedbacks { get; set; }

        public int TotalCategories { get; set; }

        public Dictionary<string, int> OrdersPerStatus { get; set; } = new();
        public List<TopSellingItemDto> TopSellingItems { get; set; } = new();
    }
}
