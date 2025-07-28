using Microsoft.EntityFrameworkCore;
using MunchKing.Context;
using MunchKing.Models;

namespace MunchKing.Repositories
{
    public class FoodItemRepository : IFoodItemRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public FoodItemRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<FoodItem>> GetAllAsync()
        {
            return await _dbContext.FoodItems.Include(f => f.Category).ToListAsync();
        }

        public async Task<FoodItem?> GetByIdAsync(int id)
        {
            return await _dbContext.FoodItems.Include(f => f.Category).FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task AddAsync(FoodItem foodItem)
        {
            await _dbContext.FoodItems.AddAsync(foodItem);
        }

        public void Update(FoodItem foodItem)
        {
            _dbContext.FoodItems.Update(foodItem);
        }

        public void Delete(FoodItem foodItem)
        {
            _dbContext.FoodItems.Remove(foodItem);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
