namespace MunchKing.DTOs
{
    public class FoodItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;


        public bool CategoryIsActive { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
