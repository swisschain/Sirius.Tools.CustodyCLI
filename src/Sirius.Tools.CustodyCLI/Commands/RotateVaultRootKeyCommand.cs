using System.Text.Json;
using GuardianApiClient;
using Microsoft.Extensions.Logging;
using Sirius.Tools.CustodyCLI.Contracts.Vault;
using VaultApiClient;
using VaultApiClient.Models;

namespace Sirius.Tools.CustodyCLI.Commands;

public class RotateVaultRootKeyCommand : ICommand
{
    private readonly string _url;
    private readonly string _file;
    private readonly IVaultApiClient _vaultApiClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly ILogger<RotateVaultRootKeyCommand> _logger;

    public RotateVaultRootKeyCommand(
        string url,
        string file,
        IVaultApiClient vaultApiClient,
        JsonSerializerOptions jsonSerializerOptions,
        ILogger<RotateVaultRootKeyCommand> logger)
    {
        _url = url;
        _file = file;
        _vaultApiClient = vaultApiClient;
        _jsonSerializerOptions = jsonSerializerOptions;
        _logger = logger;
    }

    public async Task<int> ExecuteAsync()
    {
        if (!File.Exists(_file))
        {
            _logger.LogError($"\"{_file}\" file not found.");
            return 0;
        }

        var json = await File.ReadAllTextAsync(_file);

        try
        {
            var rootKeyConfiguration = JsonSerializer.Deserialize<RootKeyConfiguration>(json, _jsonSerializerOptions);

            await _vaultApiClient.RootKeyRotationApi.RotateAsync(new RootKeyRotationModel
            {
                TenantName = rootKeyConfiguration.TenantName,
                Threshold = rootKeyConfiguration.Threshold,
                RootKeyHolders = rootKeyConfiguration.RootKeyHolders
                    .Select(o => new RootKeyHolderModel { Id = o.Id, Name = o.Name })
                    .ToList()
            });
        }
        catch (ApiException exception)
        {
            _logger.LogError(exception, exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while rotate vault root key");
        }

        _logger.LogInformation("Vault root key rotation successfully done");

        return 0;
    }
}
