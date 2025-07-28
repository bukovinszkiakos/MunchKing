using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using MunchKing.Context;
using MunchKing.Models;
using MunchKing.Contracts;
using MunchKing.DTOs;

namespace MunchKing.IntegrationTests.Controllers
{
    public class AdminContactControllerTests
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
            db.ContactMessages.RemoveRange(db.ContactMessages);
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
        public async Task GetAll_ShouldReturnContactMessages()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.ContactMessages.Add(new ContactMessage
            {
                Name = "John",
                Email = "john@example.com",
                Subject = "Test Subject",
                Message = "Test Message",
                SentAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();

            var response = await _client.GetAsync("/api/admin/contacts");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var data = await response.Content.ReadFromJsonAsync<List<ContactMessageDto>>();
            Assert.That(data, Is.Not.Null);
            Assert.That(data!.Count, Is.EqualTo(1));
            Assert.That(data[0].Email, Is.EqualTo("john@example.com"));
        }
    }
}
