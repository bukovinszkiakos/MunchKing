using MunchKing.DTOs;

namespace MunchKing.Services.Admin
{
    public interface IAdminService
    {
        Task<List<UserDto>> GetAllUsersAsync();

        Task<bool> DeleteUserAsync(string userId);

    }
}
