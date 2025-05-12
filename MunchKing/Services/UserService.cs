using Microsoft.AspNetCore.Identity;
using MunchKing.DTOs;
using MunchKing.Services;
using MunchKing.DTOs;
using MunchKing.Models;

namespace MunchKing.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();

            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName ?? "",
                    Email = user.Email ?? "",
                    Roles = roles.ToList(),
                    CreatedAt = user.LockoutEnd?.DateTime ?? DateTime.UtcNow 
                });
            }

            return result;
        }

        public async Task UpdateProfileAsync(string userId, UpdateProfileRequest request, IWebHostEnvironment env)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return;

            user.Email = request.Email;
            user.UserName = request.Username;
            user.FullName = request.FullName;
            user.MobileNumber = request.MobileNumber;
            user.Address = request.Address;
            user.PostalCode = request.PostalCode;

            if (request.ProfileImage != null && request.ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(env.WebRootPath, "uploads/users");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(request.ProfileImage.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.ProfileImage.CopyToAsync(stream);
                }

                user.ProfileImageUrl = $"/uploads/users/{uniqueName}";
            }

            await _userManager.UpdateAsync(user);
        }


    }
}
