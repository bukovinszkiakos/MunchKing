namespace MunchKing.Models
{
    public class FoodItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
