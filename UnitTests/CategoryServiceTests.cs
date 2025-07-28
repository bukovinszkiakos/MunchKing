using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MunchKing.DTOs;
using MunchKing.Models;
using MunchKing.Repositories;
using MunchKing.Services;
using Moq;
using NUnit.Framework;

namespace MunchKing.Tests.Services
{
    public class CategoryServiceTests
    {
        private Mock<ICategoryRepository> _repoMock;
        private CategoryService _service;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<ICategoryRepository>();
            _service = new CategoryService(_repoMock.Object);
        }

        [Test]
        public async Task GetAllAsync_Should_Return_CategoryDtos()
        {
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Pizza", ImageUrl = "/img/pizza.png", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Category { Id = 2, Name = "Burger", ImageUrl = "/img/burger.png", IsActive = false, CreatedAt = DateTime.UtcNow }
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            var result = await _service.GetAllAsync();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Pizza", result[0].Name);
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_CategoryDto_If_Found()
        {
            var category = new Category { Id = 1, Name = "Drinks", ImageUrl = "/img/drinks.png", IsActive = true, CreatedAt = DateTime.UtcNow };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.AreEqual("Drinks", result.Name);
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_Null_If_Not_Found()
        {
            _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Category?)null);

            var result = await _service.GetByIdAsync(99);

            Assert.IsNull(result);
        }

        [Test]
        public async Task CreateAsync_Should_Add_And_Return_CategoryDto()
        {
            Category? saved = null;
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Category>()))
                .Callback<Category>(c => saved = c)
                .Returns(Task.CompletedTask);

            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new CategoryDto
            {
                Name = "Wraps",
                ImageUrl = "/img/wraps.png",
                IsActive = true
            };

            var result = await _service.CreateAsync(dto);

            Assert.NotNull(saved);
            Assert.AreEqual(dto.Name, result.Name);
            Assert.AreEqual(dto.ImageUrl, result.ImageUrl);
        }

        [Test]
        public async Task UpdateAsync_Should_Update_Category_If_Found()
        {
            var category = new Category { Id = 1, Name = "Old", ImageUrl = "", IsActive = false };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new CategoryDto
            {
                Name = "Updated",
                ImageUrl = "/img/updated.png",
                IsActive = true
            };

            await _service.UpdateAsync(1, dto);

            Assert.AreEqual("Updated", category.Name);
            Assert.AreEqual("/img/updated.png", category.ImageUrl);
            Assert.IsTrue(category.IsActive);
        }

        [Test]
        public void UpdateAsync_Should_Throw_If_Not_Found()
        {
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Category?)null);

            var dto = new CategoryDto { Name = "X", ImageUrl = "", IsActive = false };

            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(999, dto));
            Assert.That(ex.Message, Is.EqualTo("Not found"));
        }

        [Test]
        public async Task DeleteAsync_Should_Delete_If_Found()
        {
            var cat = new Category { Id = 2, Name = "ToDelete" };
            _repoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(cat);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.DeleteAsync(2);

            _repoMock.Verify(r => r.Delete(cat), Times.Once);
        }

        [Test]
        public void DeleteAsync_Should_Throw_If_Not_Found()
        {
            _repoMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync((Category?)null);

            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(3));
            Assert.That(ex.Message, Is.EqualTo("Not found"));
        }
    }
}
