using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MunchKing.Context;
using MunchKing.Models;
using MunchKing.Repositories;
using NUnit.Framework;

namespace MunchKing.Tests.Repositories
{
    public class FoodItemRepositoryTests
    {
        private ApplicationDbContext _context;
        private FoodItemRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            _repository = new FoodItemRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddAsync_Should_Add_FoodItem()
        {
            var category = new Category { Name = "Pizza", CreatedAt = DateTime.UtcNow };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var foodItem = new FoodItem
            {
                Name = "Pepperoni",
                Description = "Spicy",
                Price = 1500,
                CategoryId = category.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(foodItem);
            await _repository.SaveChangesAsync();

            var saved = await _context.FoodItems.FirstOrDefaultAsync();
            Assert.IsNotNull(saved);
            Assert.AreEqual("Pepperoni", saved.Name);
        }

        [Test]
        public async Task GetAllAsync_Should_Return_FoodItems_With_Category()
        {
            var cat = new Category { Name = "Burgers", CreatedAt = DateTime.UtcNow };
            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();

            _context.FoodItems.Add(new FoodItem
            {
                Name = "Cheeseburger",
                Description = "Classic",
                Price = 1200,
                CategoryId = cat.Id,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();
            var item = result.FirstOrDefault();

            Assert.IsNotNull(item);
            Assert.AreEqual("Cheeseburger", item.Name);
            Assert.IsNotNull(item.Category);
            Assert.AreEqual("Burgers", item.Category.Name);
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_FoodItem_With_Category()
        {
            var cat = new Category { Name = "Sides", CreatedAt = DateTime.UtcNow };
            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();

            var food = new FoodItem
            {
                Name = "Fries",
                Description = "Crispy",
                Price = 500,
                CategoryId = cat.Id,
                CreatedAt = DateTime.UtcNow
            };
            _context.FoodItems.Add(food);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(food.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual("Fries", result.Name);
            Assert.IsNotNull(result.Category);
            Assert.AreEqual("Sides", result.Category.Name);
        }

        [Test]
        public async Task Update_Should_Modify_FoodItem()
        {
            var cat = new Category { Name = "Snacks", CreatedAt = DateTime.UtcNow };
            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();

            var food = new FoodItem
            {
                Name = "Nachos",
                Description = "Cheesy",
                Price = 800,
                CategoryId = cat.Id,
                CreatedAt = DateTime.UtcNow
            };
            _context.FoodItems.Add(food);
            await _context.SaveChangesAsync();

            food.Price = 1000;
            food.Description = "Extra cheese";
            _repository.Update(food);
            await _repository.SaveChangesAsync();

            var updated = await _context.FoodItems.FindAsync(food.Id);
            Assert.AreEqual(1000, updated.Price);
            Assert.AreEqual("Extra cheese", updated.Description);
        }

        [Test]
        public async Task Delete_Should_Remove_FoodItem()
        {
            var cat = new Category { Name = "Desserts", CreatedAt = DateTime.UtcNow };
            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();

            var food = new FoodItem
            {
                Name = "Ice Cream",
                Description = "Vanilla",
                Price = 600,
                CategoryId = cat.Id,
                CreatedAt = DateTime.UtcNow
            };
            _context.FoodItems.Add(food);
            await _context.SaveChangesAsync();

            _repository.Delete(food);
            await _repository.SaveChangesAsync();

            var deleted = await _context.FoodItems.FindAsync(food.Id);
            Assert.IsNull(deleted);
        }
    }
}
