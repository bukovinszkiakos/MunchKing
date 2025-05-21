using MunchKing.DTOs;
using MunchKing.Models;
using MunchKing.Repositories;

namespace MunchKing.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                ImageUrl = c.ImageUrl,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt
            }).ToList();
        }


        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _repo.GetByIdAsync(id);
            return category == null ? null : new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            };
        }


        public async Task<CategoryDto> CreateAsync(CategoryDto dto)
        {
            var cat = new Category
            {
                Name = dto.Name,
                ImageUrl = dto.ImageUrl,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };
            await _repo.AddAsync(cat);
            await _repo.SaveChangesAsync();
            return new CategoryDto
            {
                Id = cat.Id,
                Name = cat.Name,
                ImageUrl = cat.ImageUrl,
                IsActive = cat.IsActive,
                CreatedAt = cat.CreatedAt
            };
        }


        public async Task UpdateAsync(int id, CategoryDto dto)
        {
            var cat = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Not found");
            cat.Name = dto.Name;
            cat.ImageUrl = dto.ImageUrl;
            cat.IsActive = dto.IsActive;
            _repo.Update(cat);
            await _repo.SaveChangesAsync();
        }



        public async Task DeleteAsync(int id)
        {
            var cat = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Not found");
            _repo.Delete(cat);
            await _repo.SaveChangesAsync();
        }
    }
}
