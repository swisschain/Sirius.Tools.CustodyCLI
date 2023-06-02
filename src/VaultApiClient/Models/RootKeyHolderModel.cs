using System.Text.Json.Serialization;

namespace VaultApiClient.Models;

public class RootKeyHolderModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}