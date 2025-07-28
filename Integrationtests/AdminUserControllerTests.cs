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
    public class AdminUserControllerTests
    {
        private HttpClient _client;
        private MunchKingWebApplicationFactory _factory;
        private ApplicationUser _superAdmin;
        private ApplicationUser _testUser;

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

            if (!await roleManager.RoleExistsAsync("SuperAdmin"))
                await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));

            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            _superAdmin = new ApplicationUser { UserName = "superadmin", Email = "superadmin@munchking.com" };
            await userManager.CreateAsync(_superAdmin, "Super123!");
            await userManager.AddToRoleAsync(_superAdmin, "SuperAdmin");

            _testUser = new ApplicationUser { UserName = "user1", Email = "user1@example.com" };
            await userManager.CreateAsync(_testUser, "Test123!");

            var login = new AuthRequest("superadmin@munchking.com", "Super123!");
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
        public async Task GetAllUsers_ShouldReturnUserList()
        {
            var response = await _client.GetAsync("/api/admin/users");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
            Assert.That(users, Is.Not.Null);
            Assert.That(users!.Any(u => u.Email == "user1@example.com"), Is.True);
        }

        [Test]
        public async Task ToggleAdminRole_ShouldPromoteOrDemoteUser()
        {
            var response = await _client.PutAsync($"/api/admin/users/{_testUser.Id}/toggle-admin", null);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var isAdmin = await userManager.IsInRoleAsync(_testUser, "Admin");
            Assert.That(isAdmin, Is.True);
        }

        [Test]
        public async Task ToggleAdminRole_ShouldReturnBadRequest_IfInvalidId()
        {
            var response = await _client.PutAsync("/api/admin/users/invalid-id/toggle-admin", null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}
