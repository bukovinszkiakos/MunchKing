using MunchKing.Context;
using MunchKing.DTOs;
using MunchKing.Models;
using MunchKing.Services;

using Microsoft.EntityFrameworkCore;

public class ContactMessageService : IContactMessageService
{
    private readonly ApplicationDbContext _context;

    public ContactMessageService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SubmitAsync(ContactMessageDto dto)
    {
        var message = new ContactMessage
        {
            Name = dto.Name,
            Email = dto.Email,
            Subject = dto.Subject,
            Message = dto.Message,
            SentAt = DateTime.UtcNow
        };

        _context.ContactMessages.Add(message);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ContactMessageDto>> GetAllAsync()
    {
        return await _context.ContactMessages
            .OrderByDescending(m => m.SentAt)
            .Select(m => new ContactMessageDto
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Subject = m.Subject,
                Message = m.Message,
                SentAt = m.SentAt
            })
            .ToListAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var message = await _context.ContactMessages.FindAsync(id);
        if (message == null) return false;

        _context.ContactMessages.Remove(message);
        await _context.SaveChangesAsync();
        return true;
    }
}
