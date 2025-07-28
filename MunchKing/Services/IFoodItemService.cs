using MunchKing.DTOs;

namespace MunchKing.Services
{
    public interface IFoodItemService
    {
        Task<IEnumerable<FoodItemDto>> GetAllAsync();
        Task<FoodItemDto?> GetByIdAsync(int id);
        Task<FoodItemDto> CreateAsync(FoodItemDto dto);
        Task UpdateAsync(int id, FoodItemDto dto);
        Task DeleteAsync(int id);
    }
}
