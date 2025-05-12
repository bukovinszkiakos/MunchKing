using Microsoft.AspNetCore.Identity;

namespace MunchKing.Services.Authentication
{
    public interface ITokenService
    {
        string CreateToken(IdentityUser user, List<string> roles);
    }
}
