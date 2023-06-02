using Refit;
using VaultApiClient.Models;

namespace VaultApiClient.Api;

public interface IRootKeyRotationApi
{
    [Post("/api/root-keys/rotate")]
    Task RotateAsync([Body] RootKeyRotationModel model);
}