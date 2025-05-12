using System.ComponentModel.DataAnnotations;

namespace MunchKing.Contracts
{
    public record RegistrationRequest(
     [Required] string Email,
     [Required] string Username,
     [Required] string Password,
     [Required] string FullName,
     [Required] string Address,
     [Required] string PostalCode,
     [Required] string MobileNumber,
     IFormFile? ProfileImage 
 );

}
