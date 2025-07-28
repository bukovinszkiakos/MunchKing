using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MunchKing.Context;
using MunchKing.Models;
using MunchKing.DTOs;
using NUnit.Framework;
using MunchKing.Contracts;

namespace MunchKing.IntegrationTests.Controllers
{
    public class DashboardControllerTests
    {
        private HttpClient _client;
        private MunchKingWebApplicationFactory _factory;

        [SetUp]
        public async Task Setup()
        {
            _factory = new MunchKingWebApplicationFactory();
            _client = _factory.CreateClient();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Users.RemoveRange(db.Users);
            await db.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        private async Task AuthenticateAsAdminAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            var admin = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@munchking.com",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRoleAsync(admin, "Admin");

            var login = new AuthRequest("admin@munchking.com", "Admin123!");
            var response = await _client.PostAsJsonAsync("/Auth/Login", login);
            var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Token);
        }

        [Test]
        public async Task GetStats_Should_Return_Stats_For_Admin()
        {
            await AuthenticateAsAdminAsync();

            var response = await _client.GetAsync("/api/dashboard/stats");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var stats = await response.Content.ReadFromJsonAsync<AdminDashboardStatsDto>();
            Assert.That(stats, Is.Not.Null);
            Assert.That(stats!.TotalUsers, Is.GreaterThanOrEqualTo(0));
            Assert.That(stats.TotalOrders, Is.GreaterThanOrEqualTo(0));
        }
    }
}
