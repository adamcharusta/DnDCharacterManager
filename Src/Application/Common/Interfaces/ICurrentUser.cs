namespace DnDCharacterManager.Application.Common.Interfaces;

public interface ICurrentUser
{
    string? UserId { get; }
    string? UserName { get; }
}
