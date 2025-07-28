using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using MunchKing.Services;
using MunchKing.Repositories;
using MunchKing.Models;
using MunchKing.DTOs;

namespace MunchKing.Tests.Services
{
    [TestFixture]
    public class FoodItemServiceTests
    {
        private Mock<IFoodItemRepository> _foodRepoMock;
        private FoodItemService _service;

        [SetUp]
        public void Setup()
        {
            _foodRepoMock = new Mock<IFoodItemRepository>();
            _service = new FoodItemService(_foodRepoMock.Object);
        }

        [Test]
        public async Task GetAllAsync_Should_Return_Mapped_Dtos()
        {
            var foodItems = new List<FoodItem>
            {
                new FoodItem { Id = 1, Name = "Burger", Description = "Tasty", Price = 5, IsAvailable = true, CategoryId = 1, Category = new Category { Name = "Fast Food", IsActive = true }, CreatedAt = System.DateTime.UtcNow },
                new FoodItem { Id = 2, Name = "Fries", Description = "Crispy", Price = 3, IsAvailable = true, CategoryId = 1, Category = new Category { Name = "Fast Food", IsActive = true }, CreatedAt = System.DateTime.UtcNow }
            };

            _foodRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(foodItems);

            var result = await _service.GetAllAsync();

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Burger", result.First().Name);
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_Mapped_Dto()
        {
            var food = new FoodItem { Id = 1, Name = "Burger", Description = "Tasty", Price = 5, IsAvailable = true, CategoryId = 1, Category = new Category { Name = "Fast Food", IsActive = true }, CreatedAt = System.DateTime.UtcNow };
            _foodRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(food);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.AreEqual("Burger", result?.Name);
        }

        [Test]
        public async Task CreateAsync_Should_Save_And_Return_FoodItem()
        {
            var dto = new FoodItemDto { Name = "Pizza", Description = "Cheesy", Price = 8, ImageUrl = "/img/pizza.jpg", IsAvailable = true, CategoryId = 2 };
            var savedFood = new FoodItem { Id = 1, Name = dto.Name, Description = dto.Description, Price = dto.Price, CategoryId = dto.CategoryId };

            _foodRepoMock.Setup(r => r.AddAsync(It.IsAny<FoodItem>())).Callback<FoodItem>(f => f.Id = 1).Returns(Task.CompletedTask);
            _foodRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _foodRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(savedFood);

            var result = await _service.CreateAsync(dto);

            Assert.NotNull(result);
            Assert.AreEqual("Pizza", result.Name);
        }

        [Test]
        public async Task UpdateAsync_Should_Update_FoodItem()
        {
            var existing = new FoodItem { Id = 1, Name = "Burger", Description = "Old", Price = 4, CategoryId = 1 };
            var updated = new FoodItemDto { Name = "Burger Deluxe", Description = "New", Price = 6, ImageUrl = "/img/burger.jpg", IsAvailable = true, CategoryId = 1 };

            _foodRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _foodRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.UpdateAsync(1, updated);

            Assert.AreEqual("Burger Deluxe", existing.Name);
            Assert.AreEqual("New", existing.Description);
            Assert.AreEqual(6, existing.Price);
        }

        [Test]
        public async Task DeleteAsync_Should_Remove_FoodItem()
        {
            var food = new FoodItem { Id = 1, Name = "Burger" };
            _foodRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(food);
            _foodRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.DeleteAsync(1);

            _foodRepoMock.Verify(r => r.Delete(food), Times.Once);
            _foodRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
