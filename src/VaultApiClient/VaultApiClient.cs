using Refit;
using VaultApiClient.Api;

namespace VaultApiClient;

public class VaultApiClient : IVaultApiClient
{
    public VaultApiClient(string url)
    {
        var settings = new RefitSettings();

        RootKeyInitializationApi = RestService.For<IRootKeyInitializationApi>(url, settings);
        RootKeyRotationApi = RestService.For<IRootKeyRotationApi>(url, settings);
    }

    public IRootKeyInitializationApi RootKeyInitializationApi { get; }
    public IRootKeyRotationApi RootKeyRotationApi { get; }
}