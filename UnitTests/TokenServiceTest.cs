using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MunchKing.Services.Authentication;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MunchKing.Tests.Services
{
    public class TokenServiceTests
    {
        private TokenService _tokenService;
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                { "Jwt:ValidIssuer", "MunchKingTestIssuer" },
                { "Jwt:ValidAudience", "MunchKingTestAudience" },
                { "Jwt:IssuerSigningKey", "supersecretkey_for_testing_123456!" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _tokenService = new TokenService(_configuration);
        }

        [Test]
        public void CreateToken_Should_Generate_Valid_Jwt()
        {
            var user = new IdentityUser
            {
                Id = "test-user-id",
                UserName = "testuser",
                Email = "test@example.com"
            };

            var roles = new List<string> { "User", "Admin" };

            var token = _tokenService.CreateToken(user, roles);

            Assert.IsNotNull(token);
            Assert.IsNotEmpty(token);

            var handler = new JwtSecurityTokenHandler();
            Assert.IsTrue(handler.CanReadToken(token));

            var jwt = handler.ReadJwtToken(token);

            Assert.AreEqual("MunchKingTestIssuer", jwt.Issuer);
            Assert.AreEqual("MunchKingTestAudience", jwt.Audiences.First());

            var claims = jwt.Claims;
            Assert.IsTrue(claims.Any(c => c.Type == ClaimTypes.Name && c.Value == "testuser"));
            Assert.IsTrue(claims.Any(c => c.Type == ClaimTypes.Email && c.Value == "test@example.com"));
            Assert.IsTrue(claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "User"));
            Assert.IsTrue(claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin"));
        }
    }
}
