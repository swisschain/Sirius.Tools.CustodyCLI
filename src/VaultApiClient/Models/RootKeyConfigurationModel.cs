using System.Text.Json.Serialization;

namespace VaultApiClient.Models;

public class RootKeyConfigurationModel
{
    [JsonPropertyName("tenantName")]
    public string TenantName { get; set; } = null!;

    [JsonPropertyName("rootKeyHolders")]
    public IReadOnlyCollection<RootKeyHolderModel> RootKeyHolders { get; set; } = null!;

    [JsonPropertyName("threshold")]
    public int Threshold { get; set; }
}