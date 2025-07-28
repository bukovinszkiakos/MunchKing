using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MunchKing.Context;
using MunchKing.DTOs;
using MunchKing.Models;
using MunchKing.Services;
using NUnit.Framework;

namespace MunchKing.Tests.Services
{
    public class ContactMessageServiceTests
    {
        private ApplicationDbContext _context;
        private ContactMessageService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            _service = new ContactMessageService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task SubmitAsync_Should_Save_Message()
        {
            var dto = new ContactMessageDto
            {
                Name = "John Doe",
                Email = "john@example.com",
                Subject = "Test Subject",
                Message = "Test message content"
            };

            await _service.SubmitAsync(dto);

            var saved = await _context.ContactMessages.FirstOrDefaultAsync();

            Assert.NotNull(saved);
            Assert.AreEqual(dto.Name, saved.Name);
            Assert.AreEqual(dto.Email, saved.Email);
            Assert.AreEqual(dto.Subject, saved.Subject);
            Assert.AreEqual(dto.Message, saved.Message);
            Assert.That(saved.SentAt, Is.Not.EqualTo(default(DateTime)));
        }

        [Test]
        public async Task GetAllAsync_Should_Return_All_Messages()
        {
            _context.ContactMessages.AddRange(
                new ContactMessage { Name = "A", Email = "a@test.com", Subject = "S1", Message = "M1", SentAt = DateTime.UtcNow.AddMinutes(-5) },
                new ContactMessage { Name = "B", Email = "b@test.com", Subject = "S2", Message = "M2", SentAt = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            var result = await _service.GetAllAsync();

            Assert.AreEqual(2, result.Count);
            Assert.That(result[0].Name, Is.EqualTo("B")); 
            Assert.That(result[1].Name, Is.EqualTo("A"));
        }

        [Test]
        public async Task DeleteAsync_Should_Remove_Message_If_Exists()
        {
            var message = new ContactMessage { Name = "Delete", Email = "d@test.com", Subject = "Del", Message = "To be deleted", SentAt = DateTime.UtcNow };
            _context.ContactMessages.Add(message);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(message.Id);

            Assert.IsTrue(result);
            Assert.IsEmpty(_context.ContactMessages.ToList());
        }

        [Test]
        public async Task DeleteAsync_Should_Return_False_If_Message_Not_Found()
        {
            var result = await _service.DeleteAsync(999); 

            Assert.IsFalse(result);
        }
    }
}
