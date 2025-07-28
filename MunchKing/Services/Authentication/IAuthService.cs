using MunchKing.Contracts;

namespace MunchKing.Services.Authentication
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegistrationRequest request, string role = "User");
        Task<AuthResult> LoginAsync(string email, string password);


    }
}
