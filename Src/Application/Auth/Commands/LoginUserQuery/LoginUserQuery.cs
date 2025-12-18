using Application.Common.Dtos;
using Application.Common.Services;

namespace DnDCharacterManager.Application.Auth.Commands.LoginUser;

public record LoginUserQuery(string Email, string Password, bool RememberMe = false);

public static class LoginUserQueryHandler
{
    public static async Task<LoginResult> Handle(LoginUserQuery command, IAuthService authService)
    {
        var result = await authService.LoginAsync(command.Email, command.Password);

        return result;
    }
}
