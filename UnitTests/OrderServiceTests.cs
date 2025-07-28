using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MunchKing.Context;
using MunchKing.DTOs;
using MunchKing.Enums;
using MunchKing.Models;
using MunchKing.Services;
using NUnit.Framework;
using static MunchKing.DTOs.CheckoutRequest;

namespace MunchKing.Tests.Services
{
    public class OrderServiceTests
    {
        private ApplicationDbContext _context;
        private OrderService _orderService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            _orderService = new OrderService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task PlaceOrderAsync_Should_Save_Order_And_Return_Id()
        {
            var userId = "user1";
            var food = new FoodItem
            {
                Id = 1,
                Name = "Burger",
                Description = "Tasty burger",
                Price = 10.0m,
                ImageUrl = "/img/burger.jpg",
                IsAvailable = true,
                CategoryId = 1
            };
            _context.FoodItems.Add(food);
            await _context.SaveChangesAsync();

            var request = new CheckoutRequest
            {
                PaymentMode = "CASH",
                Items = new List<CartItem>
        {
            new CartItem { FoodItemId = 1, Quantity = 2 }
        }
            };

            var orderId = await _orderService.PlaceOrderAsync(userId, request);
            var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == orderId);

            Assert.NotNull(order);
            Assert.AreEqual(1, order.OrderItems.Count);
            Assert.AreEqual(20.0m, order.TotalAmount);
        }


        [Test]
        public async Task GetUserOrdersAsync_Should_Return_Only_That_User_Orders()
        {
            var food = new FoodItem
            {
                Id = 1,
                Name = "Fries",
                Description = "Crispy fries",
                Price = 5.0m,
                ImageUrl = "/img/fries.jpg",
                IsAvailable = true,
                CategoryId = 1
            };
            _context.FoodItems.Add(food);
            await _context.SaveChangesAsync();

            _context.Orders.AddRange(
                new Order
                {
                    UserId = "user1",
                    CreatedAt = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    PaymentMode = "CARD",
                    OrderItems = new List<OrderItem>
                    {
                new OrderItem { FoodItemId = 1, Quantity = 2, UnitPrice = 5.0m, FoodItem = food }
                    }
                },
                new Order
                {
                    UserId = "user2",
                    CreatedAt = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    PaymentMode = "CARD",
                    OrderItems = new List<OrderItem>
                    {
                new OrderItem { FoodItemId = 1, Quantity = 1, UnitPrice = 5.0m, FoodItem = food }
                    }
                }
            );
            await _context.SaveChangesAsync();

            var result = await _orderService.GetUserOrdersAsync("user1");

            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.All(o => o.Items.Sum(i => i.Quantity * i.UnitPrice) == 10));
        }


        [Test]
        public async Task UpdateOrderStatusAsync_Should_Update_Status()
        {
            var order = new Order
            {
                UserId = "user",
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                PaymentMode = "CARD",
                OrderItems = new List<OrderItem>()
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var result = await _orderService.UpdateOrderStatusAsync(order.Id, "Completed");
            var updated = await _context.Orders.FindAsync(order.Id);

            Assert.IsTrue(result);
            Assert.AreEqual(OrderStatus.Completed, updated.Status);
        }

        [Test]
        public async Task GetAllOrdersAsync_Should_Return_All_Orders()
        {
            _context.Orders.Add(new Order { UserId = "user", CreatedAt = DateTime.UtcNow, PaymentMode = "CARD", Status = OrderStatus.Pending });
            _context.Orders.Add(new Order { UserId = "user2", CreatedAt = DateTime.UtcNow, PaymentMode = "CASH", Status = OrderStatus.Completed });
            await _context.SaveChangesAsync();

            var result = await _orderService.GetAllOrdersAsync();

            Assert.AreEqual(2, result.Count);
        }
    }
}
