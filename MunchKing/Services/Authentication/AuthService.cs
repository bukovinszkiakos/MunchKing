using Microsoft.AspNetCore.Identity;
using MunchKing.Contracts;
using MunchKing.Models;

namespace MunchKing.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IWebHostEnvironment _env;

        public AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _env = env;
        }

        public async Task<AuthResult> RegisterAsync(RegistrationRequest request, string role = "User")
        {
            string? imageUrl = null;
            if (request.ProfileImage != null && request.ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/users");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(request.ProfileImage.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.ProfileImage.CopyToAsync(stream);
                }

                imageUrl = $"/uploads/users/{uniqueName}";
            }
            else
            {
                
                imageUrl = "/uploads/users/default_profile.png"; 
            }

            var user = new ApplicationUser
            {
                UserName = request.Username,
                Email = request.Email,
                FullName = request.FullName,
                Address = request.Address,
                PostalCode = request.PostalCode,
                MobileNumber = request.MobileNumber,
                ProfileImageUrl = imageUrl
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var authResult = new AuthResult(false, request.Email, request.Username, "");
                foreach (var error in result.Errors)
                {
                    authResult.ErrorMessages.Add(error.Code, error.Description);
                }
                return authResult;
            }

            await _userManager.AddToRoleAsync(user, role);

            return new AuthResult(true, user.Email, user.UserName, "");
        }


        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var authResult = new AuthResult(false, email, "", "");
                authResult.ErrorMessages.Add("BadCredentials", "Invalid email");
                return authResult;
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
            {
                var authResult = new AuthResult(false, email, user.UserName, "");
                authResult.ErrorMessages.Add("BadCredentials", "Invalid password");
                return authResult;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.CreateToken(user, roles.ToList());

            return new AuthResult(true, user.Email, user.UserName, token);
        }
    }
}