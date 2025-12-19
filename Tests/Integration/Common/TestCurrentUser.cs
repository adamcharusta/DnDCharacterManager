using DnDCharacterManager.Application.Common.Interfaces;

namespace DnDCharacterManager.Tests.Integration.Common;

public sealed class TestCurrentUser : ICurrentUser
{
    public string? UserId => "integration-test-user";

    public string? UserName => "integration-test";
}
