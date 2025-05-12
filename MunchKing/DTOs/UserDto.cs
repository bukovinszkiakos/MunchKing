namespace MunchKing.DTOs
{
    public class UserDto
    {
        public string Id { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public List<string> Roles { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
