using Microsoft.EntityFrameworkCore;
using MunchKing.Context;
using MunchKing.Models;
using MunchKing.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MunchKing.Tests.Repositories
{
    public class CategoryRepositoryTests
    {
        private ApplicationDbContext _context;
        private CategoryRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            _repository = new CategoryRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddAsync_Should_Add_Category()
        {
            var category = new Category { Name = "Burgers", IsActive = true, CreatedAt = DateTime.UtcNow };

            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync();

            var saved = await _context.Categories.FirstOrDefaultAsync();
            Assert.IsNotNull(saved);
            Assert.AreEqual("Burgers", saved.Name);
        }

        [Test]
        public async Task GetAllAsync_Should_Return_All_Categories()
        {
            _context.Categories.AddRange(
                new Category { Name = "Pizza", IsActive = true },
                new Category { Name = "Drinks", IsActive = true }
            );
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(c => c.Name == "Pizza"));
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_Correct_Category()
        {
            var cat = new Category { Name = "Desserts", IsActive = true };
            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();

            var fetched = await _repository.GetByIdAsync(cat.Id);

            Assert.IsNotNull(fetched);
            Assert.AreEqual("Desserts", fetched.Name);
        }

        [Test]
        public async Task Update_Should_Modify_Category()
        {
            var cat = new Category { Name = "Snacks", IsActive = true };
            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();

            cat.Name = "Updated Snacks";
            _repository.Update(cat);
            await _repository.SaveChangesAsync();

            var updated = await _context.Categories.FindAsync(cat.Id);
            Assert.AreEqual("Updated Snacks", updated.Name);
        }

        [Test]
        public async Task Delete_Should_Remove_Category()
        {
            var cat = new Category { Name = "ToDelete", IsActive = false };
            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();

            _repository.Delete(cat);
            await _repository.SaveChangesAsync();

            var exists = await _context.Categories.FindAsync(cat.Id);
            Assert.IsNull(exists);
        }
    }
}
