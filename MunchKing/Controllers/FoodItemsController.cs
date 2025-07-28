using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MunchKing.DTOs;
using MunchKing.Services;

namespace MunchKing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodItemsController : ControllerBase
    {
        private readonly IFoodItemService _service;

        public FoodItemsController(IFoodItemService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [RequestSizeLimit(5_000_000)]
        public async Task<IActionResult> Create([FromForm] string name,
                                         [FromForm] string description,
                                         [FromForm] decimal price,
                                         [FromForm] int categoryId,
                                         [FromForm] bool isAvailable,
                                         [FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("Image is required.");

            var folderPath = Path.Combine("wwwroot", "uploads", "products");
            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}_{image.FileName}";
            var filePath = Path.Combine(folderPath, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(stream);

            var publicHost = Environment.GetEnvironmentVariable("PUBLIC_HOST") ?? Request.Host.Value;
            var imageUrl = $"http://{publicHost}/uploads/products/{fileName}";

            var dto = new FoodItemDto
            {
                Name = name,
                Description = description,
                Price = price,
                CategoryId = categoryId,
                IsAvailable = isAvailable,
                ImageUrl = imageUrl
            };

            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }


        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        [RequestSizeLimit(5_000_000)]
        public async Task<IActionResult> Update(int id,
                                                [FromForm] string name,
                                                [FromForm] string description,
                                                [FromForm] decimal price,
                                                [FromForm] int categoryId,
                                                [FromForm] bool isAvailable,
                                                [FromForm] IFormFile? image)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            string imageUrl = existing.ImageUrl;

            if (image != null && image.Length > 0)
            {
                var folderPath = Path.Combine("wwwroot", "uploads", "products");
                Directory.CreateDirectory(folderPath);

                var fileName = $"{Guid.NewGuid()}_{image.FileName}";
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                var host = $"{Request.Scheme}://{Request.Host}";
                imageUrl = $"{host}/uploads/products/{fileName}";
            }

            var dto = new FoodItemDto
            {
                Id = id,
                Name = name,
                Description = description,
                Price = price,
                CategoryId = categoryId,
                IsAvailable = isAvailable,
                ImageUrl = imageUrl
            };

            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }


        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("upload-image")]
        [RequestSizeLimit(5_000_000)]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No file uploaded.");

            var folderPath = Path.Combine("wwwroot", "uploads", "products");
            Directory.CreateDirectory(folderPath);

            var uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
            var filePath = Path.Combine(folderPath, uniqueName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(stream);

            var publicHost = Environment.GetEnvironmentVariable("PUBLIC_HOST") ?? Request.Host.Value;
            var imageUrl = $"http://{publicHost}/uploads/products/{uniqueName}";

            return Ok(new { imageUrl });
        }



    }
}
