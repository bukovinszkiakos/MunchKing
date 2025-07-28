using Microsoft.AspNetCore.Identity;
using Moq;
using MunchKing.Models;
using MunchKing.Services.Authentication;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MunchKing.Tests.Services
{
    public class RoleSeederTests
    {
        private Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private RoleSeeder _roleSeeder;

        [SetUp]
        public void Setup()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);

            var userStore = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);

            _roleSeeder = new RoleSeeder(_roleManagerMock.Object, _userManagerMock.Object);
        }

        [Test]
        public async Task SeedRolesAndAdminAsync_Should_Create_Roles_And_Users_If_Not_Exist()
        {
            _roleManagerMock.Setup(r => r.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _roleManagerMock.Setup(r => r.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(u => u.FindByEmailAsync("admin@admin.com")).ReturnsAsync((ApplicationUser)null);
            _userManagerMock.Setup(u => u.FindByEmailAsync("super@admin.com")).ReturnsAsync((ApplicationUser)null);

            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            await _roleSeeder.SeedRolesAndAdminAsync();

            _roleManagerMock.Verify(r => r.CreateAsync(It.Is<IdentityRole>(role => role.Name == "SuperAdmin")), Times.Once);
            _roleManagerMock.Verify(r => r.CreateAsync(It.Is<IdentityRole>(role => role.Name == "Admin")), Times.Once);
            _roleManagerMock.Verify(r => r.CreateAsync(It.Is<IdentityRole>(role => role.Name == "User")), Times.Once);

            _userManagerMock.Verify(u => u.CreateAsync(It.Is<ApplicationUser>(u => u.Email == "admin@admin.com"), "Admin123!"), Times.Once);
            _userManagerMock.Verify(u => u.AddToRoleAsync(It.Is<ApplicationUser>(u => u.Email == "admin@admin.com"), "Admin"), Times.Once);

            _userManagerMock.Verify(u => u.CreateAsync(It.Is<ApplicationUser>(u => u.Email == "super@admin.com"), "Super123!"), Times.Once);
            _userManagerMock.Verify(u => u.AddToRoleAsync(It.Is<ApplicationUser>(u => u.Email == "super@admin.com"), "SuperAdmin"), Times.Once);
        }

        [Test]
        public async Task SeedRolesAndAdminAsync_Should_Not_Create_Users_If_Already_Exists()
        {
            _roleManagerMock.Setup(r => r.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            _userManagerMock.Setup(u => u.FindByEmailAsync("admin@admin.com")).ReturnsAsync(new ApplicationUser());
            _userManagerMock.Setup(u => u.FindByEmailAsync("super@admin.com")).ReturnsAsync(new ApplicationUser());

            await _roleSeeder.SeedRolesAndAdminAsync();

            _roleManagerMock.Verify(r => r.CreateAsync(It.IsAny<IdentityRole>()), Times.Never);
            _userManagerMock.Verify(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
            _userManagerMock.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }
    }
}
