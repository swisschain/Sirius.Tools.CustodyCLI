namespace VaultApiClient;

public class VaultApiClientFactory : IVaultApiClientFactory
{
    public IVaultApiClient Create(string url)
    {
        return new VaultApiClient(url);
    }
}
