using Application.Common.Dtos;
using DnDCharacterManager.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class AuthService(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager) : IAuthService
{
    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        var result = await signInManager.PasswordSignInAsync(
            email, password, false, false);

        return result.Succeeded
            ? new LoginResult(true, null)
            : new LoginResult(false, "Invalid credentials");
    }

    public async Task LogoutAsync()
    {
        await signInManager.SignOutAsync();
    }

    public async Task<RegisterResult> RegisterAsync(string email, string password)
    {
        var user = new ApplicationUser { UserName = email, Email = email };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return new RegisterResult(false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await signInManager.SignInAsync(user, false);
        return new RegisterResult(true, null);
    }
}
