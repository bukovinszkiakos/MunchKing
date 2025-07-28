using MunchKing.DTOs;

public interface IAdminStatsService
{
    Task<SalesSummaryDto> GetSalesSummaryAsync();
    Task<List<TopProductDto>> GetTopSellingItemsAsync();
}
