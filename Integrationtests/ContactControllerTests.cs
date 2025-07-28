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
    public class ContactControllerTests
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
            db.ContactMessages.RemoveRange(db.ContactMessages);
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

            var adminUser = await userManager.FindByEmailAsync("admin@munchking.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@munchking.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            var login = new AuthRequest("admin@munchking.com", "Admin123!");
            var response = await _client.PostAsJsonAsync("/Auth/Login", login);
            var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Token);
        }

        [Test]
        public async Task SendMessage_Should_Save_And_Return_Ok()
        {
            var message = new ContactMessageDto
            {
                Name = "Test User",
                Email = "test@example.com",
                Subject = "Hello",
                Message = "This is a test message"
            };

            var response = await _client.PostAsJsonAsync("/api/contact", message);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            Assert.That(json, Is.Not.Null);
            Assert.That(json!["message"], Is.EqualTo("Your message has been sent successfully."));
        }

        [Test]
        public async Task GetAllMessages_Should_Return_List_For_Admin()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.ContactMessages.Add(new ContactMessage
                {
                    Name = "Admin",
                    Email = "admin@example.com",
                    Subject = "Admin Inquiry",
                    Message = "Admin message",
                    SentAt = DateTime.UtcNow
                });
                await db.SaveChangesAsync();
            }

            await AuthenticateAsAdminAsync();

            var response = await _client.GetAsync("/api/contact/admin/all");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var data = await response.Content.ReadFromJsonAsync<List<ContactMessageDto>>();
            Assert.That(data, Is.Not.Null);
            Assert.That(data!.Count, Is.EqualTo(1));
            Assert.That(data[0].Email, Is.EqualTo("admin@example.com"));
        }

        [Test]
        public async Task DeleteMessage_Should_Remove_Message()
        {
            int messageId;

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var message = new ContactMessage
                {
                    Name = "Delete Me",
                    Email = "delete@example.com",
                    Subject = "Delete",
                    Message = "Please delete me",
                    SentAt = DateTime.UtcNow
                };
                db.ContactMessages.Add(message);
                await db.SaveChangesAsync();
                messageId = message.Id;
            }

            await AuthenticateAsAdminAsync();

            var response = await _client.DeleteAsync($"/api/contact/admin/delete/{messageId}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            Assert.That(json, Is.Not.Null);
            Assert.That(json!["message"], Is.EqualTo("Message deleted successfully."));
        }
    }
}
