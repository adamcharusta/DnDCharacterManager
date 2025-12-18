using Microsoft.AspNetCore.Antiforgery;

namespace DnDCharacterManager.Web.Common.Metadata;

public sealed class DisableAntiforgery : IAntiforgeryMetadata
{
    public bool RequiresValidation => false;
}
