using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MunchKing.DTOs;
using MunchKing.Contracts;
using MunchKing.Models;
using System.Security.Claims;
using MunchKing.Services;

namespace MunchKing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IUserService _userService;

        public ProfileController(IUserService userService, IWebHostEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _env = env;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<ProfileDto>> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            var host = Environment.GetEnvironmentVariable("PUBLIC_HOST") ?? "localhost:5136";
            var fullImageUrl = string.IsNullOrEmpty(user.ProfileImageUrl)
                ? null
                : $"http://{host}{user.ProfileImageUrl}";

            return new ProfileDto(
                user.Email!,
                user.UserName!,
                user.FullName,
                fullImageUrl,
                user.MobileNumber,
                user.Address,
                user.PostalCode
            );
        }



        [HttpPost("update")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await _userService.UpdateProfileAsync(userId, request, _env);
                return Ok(new { message = "Profile updated" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });

            }
        }



    }
}
