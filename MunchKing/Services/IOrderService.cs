using MunchKing.DTOs;
using MunchKing.Models;

namespace MunchKing.Services
{
    public interface IOrderService
    {
        Task<int> PlaceOrderAsync(string userId, CheckoutRequest request);
        Task<List<OrderDto>> GetUserOrdersAsync(string userId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus);
        Task<List<AdminOrderDto>> GetAllOrdersForAdminAsync(string? status = null);
        Task<List<OrderDto>> GetAllOrdersAsync();


    }
}
