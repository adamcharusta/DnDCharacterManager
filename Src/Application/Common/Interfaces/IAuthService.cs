using Application.Common.Dtos;

namespace DnDCharacterManager.Application.Common.Interfaces;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(string email, string password);
    Task LogoutAsync();
    Task<RegisterResult> RegisterAsync(string email, string password);
}
