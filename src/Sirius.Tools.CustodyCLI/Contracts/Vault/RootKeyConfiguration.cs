namespace Sirius.Tools.CustodyCLI.Contracts.Vault;

public class RootKeyConfiguration
{
    public string TenantName { get; set; } = null!;

    public IReadOnlyCollection<RootKeyHolder> RootKeyHolders { get; set; } = null!;

    public int Threshold { get; set; }
}
