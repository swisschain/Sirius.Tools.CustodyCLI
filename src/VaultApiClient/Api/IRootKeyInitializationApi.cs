using Refit;
using VaultApiClient.Models;

namespace VaultApiClient.Api;

public interface IRootKeyInitializationApi
{
    [Post("/api/root-keys/initialize")]
    Task ConfigureAsync([Body] RootKeyConfigurationModel model);
}