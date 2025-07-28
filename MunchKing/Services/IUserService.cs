using MunchKing.DTOs;

namespace MunchKing.Services
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();

        Task UpdateProfileAsync(string userId, UpdateProfileRequest request, IWebHostEnvironment env);

        Task<bool> ToggleAdminRoleAsync(string userId);


    }
}
