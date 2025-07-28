using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MunchKing.Context;
using MunchKing.Enums;
using MunchKing.Models;
using MunchKing.Services;
using NUnit.Framework;

namespace MunchKing.Tests.Services
{
    public class AdminStatsServiceTests
    {
        private ApplicationDbContext _context;
        private AdminStatsService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            _service = new AdminStatsService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetSalesSummaryAsync_Should_Calculate_Total_When_TotalAmount_Present()
        {
            _context.Orders.Add(new Order
            {
                UserId = "user1",
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Completed,
                TotalAmount = 100,
                PaymentMode = "CARD",
                OrderItems = new List<OrderItem>()
            });
            await _context.SaveChangesAsync();

            var summary = await _service.GetSalesSummaryAsync();

            Assert.AreEqual(1, summary.TotalOrders);
            Assert.AreEqual(100, summary.TotalRevenue);
        }

        [Test]
        public async Task GetSalesSummaryAsync_Should_Calculate_Total_When_Missing_TotalAmount()
        {
            var food = new FoodItem { Name = "Pizza", Description = "Delicious", Price = 10 };
            _context.FoodItems.Add(food);

            var order = new Order
            {
                UserId = "user1",
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Completed,
                PaymentMode = "CASH",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Quantity = 2,
                        UnitPrice = 10,
                        FoodItem = food
                    }
                }
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var summary = await _service.GetSalesSummaryAsync();

            Assert.AreEqual(1, summary.TotalOrders);
            Assert.AreEqual(20, summary.TotalRevenue);
        }

        [Test]
        public async Task GetTopSellingItemsAsync_Should_Return_Top_Products()
        {
            var food1 = new FoodItem { Name = "Burger", Description = "Tasty", Price = 5 };
            var food2 = new FoodItem { Name = "Fries", Description = "Crispy", Price = 3 };
            _context.FoodItems.AddRange(food1, food2);

            var order = new Order
            {
                UserId = "user1",
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Completed,
                PaymentMode = "CASH",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { FoodItem = food1, Quantity = 5, UnitPrice = 5 },
                    new OrderItem { FoodItem = food2, Quantity = 2, UnitPrice = 3 }
                }
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var topItems = await _service.GetTopSellingItemsAsync();

            Assert.AreEqual(2, topItems.Count);
            Assert.AreEqual("Burger", topItems[0].FoodName);
            Assert.AreEqual(5, topItems[0].QuantitySold);
        }
    }
}
