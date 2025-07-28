using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MunchKing.Services;

namespace MunchKing.Controllers
{
    [ApiController]
    [Route("api/admin/contacts")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminContactController : ControllerBase
    {
        private readonly IContactMessageService _contactService;

        public AdminContactController(IContactMessageService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var messages = await _contactService.GetAllAsync();
            return Ok(messages);
        }
    }
}
