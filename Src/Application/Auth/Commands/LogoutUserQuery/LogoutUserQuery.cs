using Application.Common.Services;

namespace DnDCharacterManager.Application.Auth.Commands.LogoutUserQuery;

public record LogoutUserQuery;

public static class LogoutUserQueryHandler
{
    public static async Task Handle(
        LogoutUserQuery query,
        IAuthService authService)
    {
        await authService.LogoutAsync();
    }
}
