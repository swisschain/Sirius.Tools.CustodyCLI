using VaultApiClient.Api;

namespace VaultApiClient;

public interface IVaultApiClient
{
    IRootKeyInitializationApi RootKeyInitializationApi { get; }
    
    IRootKeyRotationApi RootKeyRotationApi { get; }
}