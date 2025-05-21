namespace MunchKing.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? ImageUrl { get; set; } 
        public bool IsActive { get; set; } = true; 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

        public ICollection<FoodItem> FoodItems { get; set; } = new List<FoodItem>();
    }
}
