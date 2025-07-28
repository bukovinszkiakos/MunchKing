using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MunchKing.Context;
using MunchKing.Contracts;
using MunchKing.DTOs;
using MunchKing.Models;
using NUnit.Framework;

namespace MunchKing.IntegrationTests.Controllers
{
    [TestFixture]
    public class FoodItemsControllerTests
    {
        private MunchKingWebApplicationFactory _factory;
        private HttpClient _client;

        [SetUp]
        public async Task Setup()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            _factory = new MunchKingWebApplicationFactory();

            _client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                HandleCookies = true,
                AllowAutoRedirect = false
            });

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Users.RemoveRange(db.Users);
            db.FoodItems.RemoveRange(db.FoodItems);
            db.Categories.RemoveRange(db.Categories);

            db.Categories.Add(new Category
            {
                Id = 1,
                Name = "Test Category",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            await db.SaveChangesAsync();

            var content = new MultipartFormDataContent
            {
                { new StringContent("admin@example.com"), "Email" },
                { new StringContent("admin"), "Username" },
                { new StringContent("Admin123!"), "Password" },
                { new StringContent("Admin"), "FullName" },
                { new StringContent("Address"), "Address" },
                { new StringContent("1234"), "PostalCode" },
                { new StringContent("123456789"), "MobileNumber" }
            };

            var registerResponse = await _client.PostAsync("/Auth/Register", content);

            var responseBody = await registerResponse.Content.ReadAsStringAsync();
            Console.WriteLine("REGISTER STATUS: " + registerResponse.StatusCode);
            Console.WriteLine("REGISTER BODY: " + responseBody);

            Assert.That(registerResponse.IsSuccessStatusCode, Is.True, "Admin regisztráció sikertelen.");

            Assert.That(registerResponse.IsSuccessStatusCode, Is.True, "Admin regisztráció sikertelen.");

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByEmailAsync("admin@example.com");
            Assert.That(user, Is.Not.Null, "A felhasználó nem található regisztráció után.");

            await userManager.AddToRoleAsync(user, "Admin");

            var login = new AuthRequest("admin@example.com", "Admin123!");
            var loginResult = await _client.PostAsJsonAsync("/Auth/Login", login);
            Assert.That(loginResult.IsSuccessStatusCode); 
        }


        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task CreateAndGet_Should_Work()
        {
            var imageBytes = new byte[] { 1, 2, 3 };
            var imageStream = new MemoryStream(imageBytes);
            var imageContent = new StreamContent(imageStream);
            imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

            var content = new MultipartFormDataContent
    {
        { new StringContent("Test Item"), "name" },
        { new StringContent("Tasty"), "description" },
        { new StringContent("9,99"), "price" },
        { new StringContent("1"), "categoryId" },
        { new StringContent("true"), "isAvailable" },
        { imageContent, "image", "test.jpg" }
    };

            var response = await _client.PostAsync("/api/fooditems", content);
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine("RESPONSE STATUS: " + response.StatusCode);
            Console.WriteLine("RESPONSE BODY: " + body);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var created = await response.Content.ReadFromJsonAsync<FoodItemDto>();
            Assert.That(created, Is.Not.Null);
            Assert.That(created!.Name, Is.EqualTo("Test Item"));
        }




        [Test]
        public async Task Update_Should_Change_Item()
        {
            var createContent = new MultipartFormDataContent
    {
        { new StringContent("Original"), "name" },
        { new StringContent("Desc"), "description" },
        { new StringContent("5,0"), "price" },
        { new StringContent("1"), "categoryId" },
        { new StringContent("true"), "isAvailable" },
        { new StreamContent(new MemoryStream(new byte[1])), "image", "original.jpg" }
    };

            var create = await _client.PostAsync("/api/fooditems", createContent);
            Assert.That(create.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var created = await create.Content.ReadFromJsonAsync<FoodItemDto>();
            Assert.That(created, Is.Not.Null);

            var updateContent = new MultipartFormDataContent
            {
                { new StringContent("Updated"), "name" },
                { new StringContent("Updated Desc"), "description" },
                { new StringContent("10,5"), "price" },
                { new StringContent("1"), "categoryId" },
                { new StringContent("false"), "isAvailable" },
                { new StreamContent(new MemoryStream(new byte[1])), "image", "updated.jpg" }
            };

            var update = await _client.PutAsync($"/api/fooditems/{created!.Id}", updateContent);
            Assert.That(update.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

            var get = await _client.GetFromJsonAsync<FoodItemDto>($"/api/fooditems/{created.Id}");
            Assert.That(get, Is.Not.Null);
            Assert.That(get!.Name, Is.EqualTo("Updated"));
            Assert.That(get!.IsAvailable, Is.False);
        }



        [Test]
        public async Task Delete_Should_Remove_Item()
        {
            var content = new MultipartFormDataContent
        {
            { new StringContent("To Delete"), "name" },
            { new StringContent("Desc"), "description" },
            { new StringContent("3,0"), "price" },
            { new StringContent("1"), "categoryId" },
            { new StringContent("true"), "isAvailable" },
            { new StreamContent(new MemoryStream(new byte[1])), "image", "delete.jpg" }
        };

            var create = await _client.PostAsync("/api/fooditems", content);
            Assert.That(create.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var created = await create.Content.ReadFromJsonAsync<FoodItemDto>();
            Assert.That(created, Is.Not.Null);

            var delete = await _client.DeleteAsync($"/api/fooditems/{created!.Id}");
            Assert.That(delete.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

            var get = await _client.GetAsync($"/api/fooditems/{created.Id}");
            Assert.That(get.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }


    }
}
