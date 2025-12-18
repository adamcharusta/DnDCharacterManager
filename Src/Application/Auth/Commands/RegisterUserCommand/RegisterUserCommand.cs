using Application.Common.Dtos;
using Application.Common.Services;

namespace DnDCharacterManager.Application.Auth.Commands.RegisterUser;

public record RegisterUserCommand(string Email, string Password);

public static class RegisterUserCommandHandler
{
    public static async Task<RegisterResult> Handle(
        RegisterUserCommand command,
        IAuthService authService)
    {
        var result = await authService.RegisterAsync(command.Email, command.Password);

        if (result.Succeeded)
        {
            await authService.LoginAsync(command.Email, command.Password);
        }

        return result;
    }
}
