using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using MunchKing.Context;
using MunchKing.DTOs;
using MunchKing.Models;
using MunchKing.Contracts;

namespace MunchKing.IntegrationTests.Controllers
{
    [TestFixture]
    public class ProfileControllerTests
    {
        private HttpClient _client;
        private MunchKingWebApplicationFactory _factory;

        [SetUp]
        public async Task Setup()
        {
            _factory = new MunchKingWebApplicationFactory();
            _client = _factory.CreateClient();

            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Users.RemoveRange(db.Users);
            await db.SaveChangesAsync();

            var user = new ApplicationUser
            {
                UserName = "testuser",
                Email = "test@example.com",
                FullName = "Test User",
                MobileNumber = "123456789",
                Address = "Test Address",
                PostalCode = "1234"
            };

            await userManager.CreateAsync(user, "Test123!");

            var login = new AuthRequest("test@example.com", "Test123!");
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
        public async Task GetProfile_Should_Return_Profile_Info()
        {
            var response = await _client.GetAsync("/api/profile");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var profile = await response.Content.ReadFromJsonAsync<ProfileDto>();
            Assert.That(profile, Is.Not.Null);
            Assert.That(profile!.Email, Is.EqualTo("test@example.com"));
            Assert.That(profile.Username, Is.EqualTo("testuser"));
            Assert.That(profile.FullName, Is.EqualTo("Test User"));
        }

        [Test]
        public async Task UpdateProfile_Should_Update_Fields()
        {
            var content = new MultipartFormDataContent
            {
                { new StringContent("Updated Name"), "FullName" },
                { new StringContent("Updated Address"), "Address" },
                { new StringContent("9999"), "PostalCode" },
                { new StringContent("UpdatedUser"), "Username" },
                { new StringContent("updated@example.com"), "Email" },
                { new StringContent("987654321"), "MobileNumber" }
            };

            var response = await _client.PostAsync("/api/profile/update", content);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var profile = await _client.GetFromJsonAsync<ProfileDto>("/api/profile");
            Assert.That(profile, Is.Not.Null);
            Assert.That(profile!.FullName, Is.EqualTo("Updated Name"));
            Assert.That(profile.Address, Is.EqualTo("Updated Address"));
            Assert.That(profile.PostalCode, Is.EqualTo("9999"));
            Assert.That(profile.Username, Is.EqualTo("UpdatedUser"));
            Assert.That(profile.Email, Is.EqualTo("updated@example.com"));
            Assert.That(profile.MobileNumber, Is.EqualTo("987654321"));
        }
    }
}
