using Application.Common.Dtos;

namespace Application.Common.Services;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(string email, string password);
    Task LogoutAsync();
    Task<RegisterResult> RegisterAsync(string email, string password);
}
