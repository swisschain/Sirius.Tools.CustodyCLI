namespace VaultApiClient;

public interface IVaultApiClientFactory
{
    IVaultApiClient Create(string url);
}
