using MunchKing.DTOs;
using MunchKing.Models;

namespace MunchKing.Services
{
    public interface IContactMessageService
    {
        Task SubmitAsync(ContactMessageDto dto);
        Task<List<ContactMessageDto>> GetAllAsync();
        Task<bool> DeleteAsync(int id);
    }


}
