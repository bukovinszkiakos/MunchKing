using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using MunchKing.Context;
using MunchKing.Models;
using MunchKing.DTOs;
using MunchKing.Contracts;

namespace MunchKing.IntegrationTests.Controllers
{
    public class AdminControllerTests
    {
        private HttpClient _client;
        private MunchKingWebApplicationFactory _factory;
        private ApplicationUser _admin;

        [SetUp]
        public async Task Setup()
        {
            _factory = new MunchKingWebApplicationFactory();
            _client = _factory.CreateClient();

            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Users.RemoveRange(db.Users);
            await db.SaveChangesAsync();

            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            _admin = new ApplicationUser { UserName = "admin", Email = "admin@munchking.com" };
            await userManager.CreateAsync(_admin, "Admin123!");
            await userManager.AddToRoleAsync(_admin, "Admin");

            var login = new AuthRequest("admin@munchking.com", "Admin123!");
            var response = await _client.PostAsJsonAsync("/Auth/Login", login);
            var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Token);
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task GetDashboardStats_ShouldReturnOkWithData()
        {
            var response = await _client.GetAsync("/api/admin/dashboard/stats");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var stats = await response.Content.ReadFromJsonAsync<AdminDashboardStatsDto>();
            Assert.That(stats, Is.Not.Null);
        }

        [Test]
        public async Task DeleteUser_ShouldRemoveUser_WhenUserExists()
        {
            using var scope1 = _factory.Services.CreateScope();
            var userManager = scope1.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = new ApplicationUser { UserName = "deleteMe", Email = "delete@example.com" };
            await userManager.CreateAsync(user, "Test123!");

            var response = await _client.DeleteAsync($"/api/admin/users/{user.Id}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            using var scope2 = _factory.Services.CreateScope();
            var verifyManager = scope2.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var deletedUser = await verifyManager.FindByIdAsync(user.Id);
            Assert.That(deletedUser, Is.Null);
        }


        [Test]
        public async Task DeleteUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            var response = await _client.DeleteAsync("/api/admin/users/nonexistent-user-id");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}
