using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Moq;
using MunchKing.Contracts;
using MunchKing.Models;
using MunchKing.Services.Authentication;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MunchKing.Tests.Services
{
    public class AuthServiceTests
    {
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<IWebHostEnvironment> _envMock;
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            _tokenServiceMock = new Mock<ITokenService>();
            _envMock = new Mock<IWebHostEnvironment>();
            _envMock.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

            _authService = new AuthService(_userManagerMock.Object, _tokenServiceMock.Object, _envMock.Object);
        }

        [Test]
        public async Task RegisterAsync_Should_Create_User_And_Return_Success()
        {
            var request = new RegistrationRequest
            {
                Email = "test@example.com",
                Username = "testuser",
                Password = "Test123!",
                FullName = "Test User",
                Address = "Test Address",
                PostalCode = "12345",
                MobileNumber = "1234567890",
                ProfileImage = null
            };

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _authService.RegisterAsync(request);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("test@example.com", result.Email);
            Assert.AreEqual("testuser", result.Username);
        }

        [Test]
        public async Task RegisterAsync_Should_Fail_When_Email_Or_Username_Already_Exists()
        {
            // Arrange
            var request = new RegistrationRequest
            {
                Email = "fail@example.com",
                Username = "testuser",
                Password = "Test123!",
                FullName = "Test User",
                Address = "Test Address",
                PostalCode = "12345",
                MobileNumber = "1234567890",
                ProfileImage = null
            };

            var identityResult = IdentityResult.Failed(new IdentityError
            {
                Code = "DuplicateEmail",
                Description = "Email already taken"
            });

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
                .ReturnsAsync(identityResult);

            var result = await _authService.RegisterAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("fail@example.com", result.Email);
            Assert.IsTrue(result.ErrorMessages.ContainsKey("DuplicateEmail"));
        }

        [Test]
        public async Task LoginAsync_Should_Return_Token_On_Success()
        {
            var email = "login@example.com";
            var password = "Test123!";
            var user = new ApplicationUser { Email = email, UserName = "loginuser" };

            _userManagerMock.Setup(um => um.FindByEmailAsync(email))
                .ReturnsAsync(user);

            _userManagerMock.Setup(um => um.CheckPasswordAsync(user, password))
                .ReturnsAsync(true);

            _userManagerMock.Setup(um => um.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "User" });

            _tokenServiceMock.Setup(ts => ts.CreateToken(user, It.IsAny<List<string>>()))
                .Returns("mocked-token");

            var result = await _authService.LoginAsync(email, password);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("mocked-token", result.Token);
            Assert.AreEqual("loginuser", result.Username);
        }

        [Test]
        public async Task LoginAsync_Should_Fail_On_Wrong_Email()
        {
            var email = "wrong@example.com";
            var password = "Test123!";

            _userManagerMock.Setup(um => um.FindByEmailAsync(email))
                .ReturnsAsync((ApplicationUser)null);

            var result = await _authService.LoginAsync(email, password);

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.ErrorMessages.ContainsKey("BadCredentials"));
        }

        [Test]
        public async Task LoginAsync_Should_Fail_On_Wrong_Password()
        {
            var email = "user@example.com";
            var password = "wrongpass";
            var user = new ApplicationUser { Email = email, UserName = "user123" };

            _userManagerMock.Setup(um => um.FindByEmailAsync(email))
                .ReturnsAsync(user);

            _userManagerMock.Setup(um => um.CheckPasswordAsync(user, password))
                .ReturnsAsync(false);

            var result = await _authService.LoginAsync(email, password);

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.ErrorMessages.ContainsKey("BadCredentials"));
        }
    }
}
