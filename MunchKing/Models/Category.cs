namespace MunchKing.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        public ICollection<FoodItem> FoodItems { get; set; } = new List<FoodItem>();
    }
}
