using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MunchKing.DTOs;
using MunchKing.Services;

namespace MunchKing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cat = await _service.GetByIdAsync(id);
            return cat == null ? NotFound() : Ok(cat);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [RequestSizeLimit(5_000_000)]
        public async Task<IActionResult> Create([FromForm] IFormFile image, [FromForm] string name, [FromForm] bool isActive)
        {
            if (image == null || image.Length == 0)
                return BadRequest("Image is required.");

            var fileName = $"{Guid.NewGuid()}_{image.FileName}";
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/categories");
            Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            var publicHost = Environment.GetEnvironmentVariable("PUBLIC_HOST") ?? Request.Host.Value;
            var imageUrl = $"http://{publicHost}/uploads/categories/{fileName}";

            var dto = new CategoryDto
            {
                Name = name,
                ImageUrl = imageUrl,
                IsActive = isActive
            };

            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        [RequestSizeLimit(5_000_000)]
        public async Task<IActionResult> Update(int id, [FromForm] IFormFile? image, [FromForm] string name, [FromForm] bool isActive)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            string imageUrl = category.ImageUrl;

            if (image != null && image.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{image.FileName}";
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/categories");
                Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                var publicHost = Environment.GetEnvironmentVariable("PUBLIC_HOST") ?? Request.Host.Value;
                imageUrl = $"http://{publicHost}/uploads/categories/{fileName}";
            }

            var dto = new CategoryDto
            {
                Id = id,
                Name = name,
                ImageUrl = imageUrl,
                IsActive = isActive
            };

            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
