namespace MunchKing.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? ImageUrl { get; set; } 
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
