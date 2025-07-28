using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using NUnit.Framework;
using MunchKing.Contracts;

namespace MunchKing.IntegrationTests.Controllers
{
    public class AuthControllerTests
    {
        private HttpClient _client;
        private MunchKingWebApplicationFactory _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new MunchKingWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task Register_Should_Return_Created()
        {
            var form = new MultipartFormDataContent
            {
                { new StringContent("authuser@example.com"), "Email" },
                { new StringContent("authuser"), "Username" },
                { new StringContent("Test123!"), "Password" },
                { new StringContent("Auth User"), "FullName" },
                { new StringContent("123 Main St"), "Address" },
                { new StringContent("1111"), "PostalCode" },
                { new StringContent("06301234567"), "MobileNumber" }
            };

            var response = await _client.PostAsync("/Auth/Register", form);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var result = await response.Content.ReadFromJsonAsync<RegistrationResponse>();
            Assert.That(result!.Email, Is.EqualTo("authuser@example.com"));
            Assert.That(result.Username, Is.EqualTo("authuser"));
        }

        [Test]
        public async Task Login_Should_Return_Token()
        {
            var form = new MultipartFormDataContent
            {
                { new StringContent("loginuser@example.com"), "Email" },
                { new StringContent("loginuser"), "Username" },
                { new StringContent("Test123!"), "Password" },
                { new StringContent("Login User"), "FullName" },
                { new StringContent("456 Login Rd"), "Address" },
                { new StringContent("2222"), "PostalCode" },
                { new StringContent("06307654321"), "MobileNumber" }
            };
            await _client.PostAsync("/Auth/Register", form);

            var login = new AuthRequest("loginuser@example.com", "Test123!");
            var response = await _client.PostAsJsonAsync("/Auth/Login", login);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            Assert.That(result!.Email, Is.EqualTo("loginuser@example.com"));
            Assert.That(result.Username, Is.EqualTo("loginuser"));
            Assert.That(result.Token, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task Me_Should_Return_UserInfo_WhenAuthenticated()
        {
            var form = new MultipartFormDataContent
            {
                { new StringContent("meuser@example.com"), "Email" },
                { new StringContent("meuser"), "Username" },
                { new StringContent("Test123!"), "Password" },
                { new StringContent("Me User"), "FullName" },
                { new StringContent("789 Me St"), "Address" },
                { new StringContent("3333"), "PostalCode" },
                { new StringContent("06309876543"), "MobileNumber" }
            };
            await _client.PostAsync("/Auth/Register", form);

            var login = new AuthRequest("meuser@example.com", "Test123!");
            var loginResponse = await _client.PostAsJsonAsync("/Auth/Login", login);
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult!.Token);

            var meResponse = await _client.GetAsync("/Auth/Me");

            Assert.That(meResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var me = await meResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            Assert.That(me!["email"]!.ToString(), Is.EqualTo("meuser@example.com"));
            Assert.That(me["username"]!.ToString(), Is.EqualTo("meuser"));
            Assert.That(me["roles"]!.ToString(), Does.Contain("User"));
        }

        [Test]
        public async Task Me_Should_Return_Unauthorized_IfNotAuthenticated()
        {
            var response = await _client.GetAsync("/Auth/Me");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task Logout_Should_ClearCookie()
        {
            var response = await _client.PostAsync("/Auth/Logout", null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            Assert.That(result!["message"], Is.EqualTo("Logout successful"));
        }
    }
}
