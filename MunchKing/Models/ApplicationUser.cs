using Microsoft.AspNetCore.Identity;

namespace MunchKing.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; } 
    }

}
