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
                    FullName = user.FullName ?? "Unknown",
                    Email = user.Email ?? "",
                    Roles = roles.ToList(),
                    IsAdmin = roles.Contains("Admin"), // ✅ itt beállítjuk
                    CreatedAt = user.CreatedAt == default ? DateTime.UtcNow : user.CreatedAt
                });
            }

            return result;
        }


        public async Task UpdateProfileAsync(string userId, UpdateProfileRequest request, IWebHostEnvironment env)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            var existingEmailUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingEmailUser != null && existingEmailUser.Id != user.Id)
                throw new InvalidOperationException("Email already exists.");

            var existingUsernameUser = await _userManager.FindByNameAsync(request.Username);
            if (existingUsernameUser != null && existingUsernameUser.Id != user.Id)
                throw new InvalidOperationException("Username already exists.");

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

                var publicHost = Environment.GetEnvironmentVariable("PUBLIC_HOST");
                user.ProfileImageUrl = $"/uploads/users/{uniqueName}";
            }
            else if (string.IsNullOrWhiteSpace(user.ProfileImageUrl))
            {
                var publicHost = Environment.GetEnvironmentVariable("PUBLIC_HOST");
                user.ProfileImageUrl = "/uploads/users/default_profile.png";
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errorMsg = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to update profile: {errorMsg}");
            }
        }





        public async Task<bool> ToggleAdminRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var removeResult = await _userManager.RemoveFromRoleAsync(user, "Admin");
                return removeResult.Succeeded;
            }
            else
            {
                var addResult = await _userManager.AddToRoleAsync(user, "Admin");
                return addResult.Succeeded;
            }
        }



    }
}
