using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using MunchKing.Models;
using MunchKing.Services.Admin;
using NUnit.Framework;
using MunchKing.DTOs;

namespace MunchKing.Tests.Services
{
    public class AdminServiceTests
    {
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private AdminService _adminService;

        [SetUp]
        public void Setup()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            _adminService = new AdminService(_userManagerMock.Object);
        }

        [Test]
        public async Task GetAllUsersAsync_Should_Return_UserDtos_With_Roles()
        {
            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = "1",
                    UserName = "testuser",
                    Email = "test@example.com",
                    CreatedAt = DateTime.UtcNow
                }
            };

            var userQueryable = users.AsQueryable().BuildMockDbSet();

            _userManagerMock.Setup(x => x.Users).Returns(userQueryable.Object);
            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "User" });

            var result = await _adminService.GetAllUsersAsync();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("testuser", result[0].Username);
            Assert.Contains("User", result[0].Roles);
        }

        [Test]
        public async Task DeleteUserAsync_Should_Return_False_If_User_Not_Found()
        {
            _userManagerMock.Setup(x => x.FindByIdAsync("123"))
                .ReturnsAsync((ApplicationUser?)null);

            var result = await _adminService.DeleteUserAsync("123");

            Assert.IsFalse(result);
        }

        [Test]
        public async Task DeleteUserAsync_Should_Return_True_On_Successful_Delete()
        {
            var user = new ApplicationUser { Id = "123" };
            _userManagerMock.Setup(x => x.FindByIdAsync("123"))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _adminService.DeleteUserAsync("123");

            Assert.IsTrue(result);
        }
    }

    public static class QueryableMock
    {
        public static Mock<Microsoft.EntityFrameworkCore.DbSet<T>> BuildMockDbSet<T>(this IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet;
        }
    }
}
