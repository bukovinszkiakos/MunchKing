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
    public class CategoriesControllerTests
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
            db.Categories.RemoveRange(db.Categories);
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
        public async Task GetAll_ShouldReturnEmptyInitially()
        {
            var response = await _client.GetAsync("/api/categories");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
            Assert.That(categories, Is.Not.Null);
            Assert.That(categories!.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task CreateCategory_ShouldReturnCreated()
        {
            var form = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(new byte[10]);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            form.Add(fileContent, "image", "test.png");
            form.Add(new StringContent("Pizza"), "name");
            form.Add(new StringContent("true"), "isActive");

            var response = await _client.PostAsync("/api/categories", form);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var created = await response.Content.ReadFromJsonAsync<CategoryDto>();
            Assert.That(created, Is.Not.Null);
            Assert.That(created!.Name, Is.EqualTo("Pizza"));
        }

        [Test]
        public async Task GetById_ShouldReturnCategory()
        {
            var form = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(new byte[10]);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            form.Add(fileContent, "image", "test.png");
            form.Add(new StringContent("Dessert"), "name");
            form.Add(new StringContent("true"), "isActive");
            var createRes = await _client.PostAsync("/api/categories", form);
            var created = await createRes.Content.ReadFromJsonAsync<CategoryDto>();

            var getRes = await _client.GetAsync($"/api/categories/{created!.Id}");
            Assert.That(getRes.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var found = await getRes.Content.ReadFromJsonAsync<CategoryDto>();
            Assert.That(found!.Name, Is.EqualTo("Dessert"));
        }

        [Test]
        public async Task DeleteCategory_ShouldReturnNoContent()
        {
            var form = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(new byte[10]);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            form.Add(fileContent, "image", "test.png");
            form.Add(new StringContent("Drinks"), "name");
            form.Add(new StringContent("true"), "isActive");
            var createRes = await _client.PostAsync("/api/categories", form);
            var created = await createRes.Content.ReadFromJsonAsync<CategoryDto>();

            var delRes = await _client.DeleteAsync($"/api/categories/{created!.Id}");
            Assert.That(delRes.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

            var getRes = await _client.GetAsync($"/api/categories/{created.Id}");
            Assert.That(getRes.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}
