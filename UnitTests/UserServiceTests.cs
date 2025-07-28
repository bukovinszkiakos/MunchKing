using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using MunchKing.DTOs;
using MunchKing.Models;
using MunchKing.Services;
using NUnit.Framework;

namespace MunchKing.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<UserManager<ApplicationUser>> _userManagerMock = null!;
        private IUserService _userService = null!;

        [SetUp]
        public void Setup()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!
            );

            _userService = new UserService(_userManagerMock.Object);
        }

        [Test]
        public async Task GetAllUsersAsync_ReturnsCorrectUsers()
        {
            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = "1",
                    UserName = "john",
                    Email = "john@example.com",
                    FullName = "John Doe",
                    CreatedAt = DateTime.UtcNow
                }
            }.AsQueryable();

            _userManagerMock.Setup(x => x.Users).Returns(users);
            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "User", "Admin" });

            var result = await _userService.GetAllUsersAsync();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Username, Is.EqualTo("john"));
            Assert.That(result[0].IsAdmin, Is.True);
            Assert.That(result[0].Roles, Contains.Item("Admin"));
        }

        [Test]
        public async Task ToggleAdminRoleAsync_AddsAdminRole_WhenNotPresent()
        {
            var user = new ApplicationUser { Id = "2" };

            _userManagerMock.Setup(x => x.FindByIdAsync("2")).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);
            _userManagerMock.Setup(x => x.AddToRoleAsync(user, "Admin"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _userService.ToggleAdminRoleAsync("2");

            Assert.That(result, Is.True);
            _userManagerMock.Verify(x => x.AddToRoleAsync(user, "Admin"), Times.Once);
        }

        [Test]
        public async Task ToggleAdminRoleAsync_RemovesAdminRole_WhenPresent()
        {
            var user = new ApplicationUser { Id = "3" };

            _userManagerMock.Setup(x => x.FindByIdAsync("3")).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.RemoveFromRoleAsync(user, "Admin"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _userService.ToggleAdminRoleAsync("3");

            Assert.That(result, Is.True);
            _userManagerMock.Verify(x => x.RemoveFromRoleAsync(user, "Admin"), Times.Once);
        }

        [Test]
        public async Task ToggleAdminRoleAsync_ReturnsFalse_WhenUserNotFound()
        {
            _userManagerMock.Setup(x => x.FindByIdAsync("not-found")).ReturnsAsync((ApplicationUser?)null);

            var result = await _userService.ToggleAdminRoleAsync("not-found");

            Assert.That(result, Is.False);
        }
    }
}
