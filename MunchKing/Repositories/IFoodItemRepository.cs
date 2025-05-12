using MunchKing.Models;

namespace MunchKing.Repositories
{
    public interface IFoodItemRepository
    {
        Task<IEnumerable<FoodItem>> GetAllAsync();
        Task<FoodItem?> GetByIdAsync(int id);
        Task AddAsync(FoodItem foodItem);
        void Update(FoodItem foodItem);
        void Delete(FoodItem foodItem);
        Task SaveChangesAsync();
    }
}
