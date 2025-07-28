using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MunchKing.Context;
using MunchKing.Enums;
using MunchKing.Models;
using MunchKing.Services;
using NUnit.Framework;

namespace MunchKing.Tests.Services
{
    public class AdminDashboardServiceTests
    {
        private ApplicationDbContext _context;
        private AdminDashboardService _dashboardService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            _dashboardService = new AdminDashboardService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetStatisticsAsync_Should_Return_Valid_Dashboard_Data()
        {
            // Arrange
            var food = new FoodItem { Name = "Burger", Price = 10, Description = "test", IsAvailable = true, CategoryId = 1 };
            _context.FoodItems.Add(food);

            var category = new Category { Name = "Fast Food", IsActive = true, CreatedAt = DateTime.UtcNow };
            _context.Categories.Add(category);

            var feedback = new ContactMessage { Name = "User", Email = "a@b.com", Subject = "S", Message = "M", SentAt = DateTime.UtcNow };
            _context.ContactMessages.Add(feedback);

            var order = new Order
            {
                UserId = "u1",
                Status = OrderStatus.Completed,
                CreatedAt = DateTime.UtcNow,
                PaymentMode = "CARD",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { FoodItem = food, Quantity = 2, UnitPrice = 10 }
                }
            };
            _context.Orders.Add(order);

            var user = new ApplicationUser { Id = "user1", UserName = "testuser" };
            var userRole = new IdentityUserRole<string> { UserId = "user1", RoleId = "2" };
            var role = new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" };

            _context.Users.Add(user);
            _context.UserRoles.Add(userRole);
            _context.Roles.Add(role);

            await _context.SaveChangesAsync();

            var result = await _dashboardService.GetStatisticsAsync();

            Assert.AreEqual(1, result.TotalOrders);
            Assert.AreEqual(1, result.TotalUsers);
            Assert.AreEqual(1, result.TotalFoodItems);
            Assert.AreEqual(20, result.TotalRevenue);
            Assert.AreEqual(1, result.TotalFeedbacks);
            Assert.AreEqual(1, result.TotalCategories);
            Assert.AreEqual(1, result.OrdersPerStatus["Completed"]);
            Assert.AreEqual("Burger", result.TopSellingItems[0].FoodName);
            Assert.AreEqual(2, result.TopSellingItems[0].QuantitySold);
        }
    }
}
