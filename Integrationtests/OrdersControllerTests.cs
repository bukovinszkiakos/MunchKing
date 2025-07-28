using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MunchKing.Context;
using MunchKing.Contracts;
using MunchKing.DTOs;
using MunchKing.Models;
using NUnit.Framework;
using static MunchKing.DTOs.CheckoutRequest;

namespace MunchKing.IntegrationTests.Controllers
{
    [TestFixture]
    public class OrdersControllerTests
    {
        private MunchKingWebApplicationFactory _factory;
        private HttpClient _client;

        [SetUp]
        public async Task Setup()
        {
            _factory = new MunchKingWebApplicationFactory();
            _client = _factory.CreateClient();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            db.Users.RemoveRange(db.Users);
            db.FoodItems.RemoveRange(db.FoodItems);
            db.Categories.RemoveRange(db.Categories);
            db.Orders.RemoveRange(db.Orders);
            await db.SaveChangesAsync();

            var user = new ApplicationUser { UserName = "user", Email = "user@munchking.com" };
            await userManager.CreateAsync(user, "User123!");

            db.Categories.Add(new Category
            {
                Id = 1,
                Name = "Pizza",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            db.FoodItems.Add(new FoodItem
            {
                Id = 1,
                Name = "Margherita",
                Description = "Classic",
                Price = 8,
                CategoryId = 1,
                IsAvailable = true,
                ImageUrl = "http://localhost/uploads/margherita.jpg"
            });

            await db.SaveChangesAsync();

            var login = new AuthRequest("user@munchking.com", "User123!");
            var response = await _client.PostAsJsonAsync("/Auth/Login", login);
            var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth!.Token);
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task Checkout_Should_CreateOrder()
        {
            var checkout = new CheckoutRequest
            {
                PaymentMode = "CARD",
                Items = new List<CartItem>
                {
                    new CartItem { FoodItemId = 1, Quantity = 2 }
                }
            };

            var response = await _client.PostAsJsonAsync("/api/orders/checkout", checkout);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ContainsKey("orderId"), Is.True);
        }

        [Test]
        public async Task GetAvailableStatuses_Should_Return_StatusList()
        {
            var response = await _client.GetAsync("/api/orders/available-statuses");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var statuses = await response.Content.ReadFromJsonAsync<List<string>>();
            Assert.That(statuses, Is.Not.Null);
            Assert.That(statuses!.Count, Is.GreaterThan(0));
            Assert.That(statuses, Does.Contain("Pending"));
        }
    }
}