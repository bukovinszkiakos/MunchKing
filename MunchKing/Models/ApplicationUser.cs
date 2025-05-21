using Microsoft.AspNetCore.Identity;

namespace MunchKing.Models
{
    public class ApplicationUser : IdentityUser
    {
        
            public string? FullName { get; set; }
            public string? MobileNumber { get; set; }
            public string? Address { get; set; }
            public string? PostalCode { get; set; }
            public string? ProfileImageUrl { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
        

    }

}
