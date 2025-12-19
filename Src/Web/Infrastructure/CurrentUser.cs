using System.Security.Claims;
using DnDCharacterManager.Application.Common.Interfaces;

namespace DnDCharacterManager.Web.Infrastructure;

public sealed class CurrentUser(IHttpContextAccessor http) : ICurrentUser
{
    public string? UserName =>
        http.HttpContext?.User?.Identity?.Name;

    public string? UserId =>
        http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
