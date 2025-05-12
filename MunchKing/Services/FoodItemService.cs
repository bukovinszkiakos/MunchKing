using MunchKing.DTOs;
using MunchKing.Models;
using MunchKing.Repositories;

namespace MunchKing.Services
{
    public class FoodItemService : IFoodItemService
    {
        private readonly IFoodItemRepository _foodRepo;

        public FoodItemService(IFoodItemRepository foodRepo) 
        {
            _foodRepo = foodRepo;
        }

        public async Task<IEnumerable<FoodItemDto>> GetAllAsync()
        {
            var items = await _foodRepo.GetAllAsync();
            return items.Select(f => new FoodItemDto
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                Price = f.Price,
                ImageUrl = f.ImageUrl,
                IsAvailable = f.IsAvailable,
                CategoryId = f.CategoryId,
                CategoryName = f.Category?.Name ?? ""
            });
        }

        public async Task<FoodItemDto?> GetByIdAsync(int id)
        {
            var f = await _foodRepo.GetByIdAsync(id);
            if (f == null) return null;

            return new FoodItemDto
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                Price = f.Price,
                ImageUrl = f.ImageUrl,
                IsAvailable = f.IsAvailable,
                CategoryId = f.CategoryId,
                CategoryName = f.Category?.Name ?? ""
            };
        }

        public async Task<FoodItemDto> CreateAsync(FoodItemDto dto)
        {
            var food = new FoodItem
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                IsAvailable = dto.IsAvailable,
                CategoryId = dto.CategoryId
            };

            await _foodRepo.AddAsync(food);
            await _foodRepo.SaveChangesAsync();

            return await GetByIdAsync(food.Id) ?? throw new Exception("Creation failed");
        }

        public async Task UpdateAsync(int id, FoodItemDto dto)
        {
            var food = await _foodRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Item not found");

            food.Name = dto.Name;
            food.Description = dto.Description;
            food.Price = dto.Price;
            food.ImageUrl = dto.ImageUrl;
            food.IsAvailable = dto.IsAvailable;
            food.CategoryId = dto.CategoryId;

            _foodRepo.Update(food);
            await _foodRepo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var food = await _foodRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Item not found");
            _foodRepo.Delete(food);
            await _foodRepo.SaveChangesAsync();
        }
    }
}
