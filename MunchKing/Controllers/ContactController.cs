using Microsoft.AspNetCore.Mvc;
using MunchKing.DTOs;
using MunchKing.Services;
using Microsoft.AspNetCore.Authorization;

namespace MunchKing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactMessageService _contactService;

        public ContactController(IContactMessageService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ContactMessageDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _contactService.SubmitAsync(request);
            return Ok(new { message = "Your message has been sent successfully." });
        }

        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetAllMessages()
        {
            var messages = await _contactService.GetAllAsync();
            return Ok(messages);
        }

        [HttpDelete("admin/delete/{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var success = await _contactService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return Ok(new { message = "Message deleted successfully." });
        }
    }
}
