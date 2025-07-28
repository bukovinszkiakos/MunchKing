using System.ComponentModel.DataAnnotations;

namespace MunchKing.Contracts
{
    public record AuthRequest(
        [Required] string Email,
        [Required] string Password
    );
}
