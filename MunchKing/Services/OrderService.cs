using Microsoft.AspNetCore.Identity;
using MunchKing.Context;
using MunchKing.DTOs;
using MunchKing.Models;
using Microsoft.EntityFrameworkCore;
using MunchKing.Enums;

namespace MunchKing.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> PlaceOrderAsync(string userId, CheckoutRequest request)
        {
            var order = new Order
            {
                UserId = userId,
                PaymentMode = request.PaymentMode,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in request.Items)
            {
                var food = await _context.FoodItems.FindAsync(item.FoodItemId);
                if (food == null) throw new Exception("Invalid food item ID");

                var orderItem = new OrderItem
                {
                    FoodItemId = item.FoodItemId,
                    Quantity = item.Quantity,
                    UnitPrice = food.Price
                };

                order.OrderItems.Add(orderItem);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order.Id;
        }

        public async Task<List<OrderDto>> GetUserOrdersAsync(string userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.FoodItem)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(o => new OrderDto
            {
                OrderId = o.Id,
                CreatedAt = o.CreatedAt,
                Status = o.Status.ToString(),
                PaymentMode = o.PaymentMode,
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductName = oi.FoodItem.Name,
                    ImageUrl = oi.FoodItem.ImageUrl,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity
                }).ToList()
            }).ToList();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            if (!Enum.TryParse<OrderStatus>(newStatus, ignoreCase: true, out var parsedStatus))
            {
                throw new ArgumentException("Invalid status value");
            }

            order.Status = parsedStatus;
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<List<AdminOrderDto>> GetAllOrdersForAdminAsync(string? status = null)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.FoodItem)
                .Include(o => o.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(o => o.Status == parsedStatus);
            }

            var orders = await query.OrderByDescending(o => o.CreatedAt).ToListAsync();

            return orders.Select(o => new AdminOrderDto
            {
                OrderId = o.Id,
                CreatedAt = o.CreatedAt,
                Status = o.Status.ToString(),
                PaymentMode = o.PaymentMode,
                UserEmail = o.User?.Email ?? "N/A",
                Username = o.User?.UserName ?? "N/A",
                TotalAmount = o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity), 
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductName = oi.FoodItem.Name,
                    ImageUrl = oi.FoodItem.ImageUrl,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            }).ToList();

        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.FoodItem)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(o => new OrderDto
            {
                OrderId = o.Id,
                CreatedAt = o.CreatedAt,
                Status = o.Status.ToString(),
                PaymentMode = o.PaymentMode,
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductName = oi.FoodItem.Name,
                    ImageUrl = oi.FoodItem.ImageUrl,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity
                }).ToList()
            }).ToList();
        }


    }
}