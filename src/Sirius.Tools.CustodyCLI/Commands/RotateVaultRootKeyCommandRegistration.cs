using System.Text.Json;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VaultApiClient;

namespace Sirius.Tools.CustodyCLI.Commands;

public class RotateVaultRootKeyCommandRegistration : ICommandRegistration
{
    private readonly CommandFactory _factory;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public RotateVaultRootKeyCommandRegistration(CommandFactory factory, JsonSerializerOptions jsonSerializerOptions)
    {
        _factory = factory;
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public void StartExecution(CommandLineApplication lineApplication)
    {
        lineApplication.Description = "Send root key rotation settings to the Vault.";
        lineApplication.HelpOption("-?|-h|--help");

        var urlOption = lineApplication.Option(
            "-u|--url <url>",
            "Vault URL",
            CommandOptionType.SingleValue);

        var fileOption = lineApplication.Option(
            "-f|--file <file>",
            "Vault root key rotation settings JSON file",
            CommandOptionType.SingleValue);

        lineApplication.OnExecute(() =>
        {
            var urlOptionValue = urlOption.Value();

            if (string.IsNullOrEmpty(urlOptionValue))
                throw new OptionInvalidException($"{nameof(urlOption)} is required.");

            var fileOptionValue = fileOption.Value();

            if (string.IsNullOrEmpty(fileOptionValue))
                throw new OptionInvalidException($"{nameof(fileOption)} is required.");

            var command = _factory.CreateCommand(serviceProvider =>
                new RotateVaultRootKeyCommand(
                    urlOptionValue,
                    fileOptionValue,
                    serviceProvider.GetRequiredService<IVaultApiClientFactory>().Create(urlOptionValue),
                    _jsonSerializerOptions,
                    serviceProvider.GetRequiredService<ILogger<RotateVaultRootKeyCommand>>()));

            return command.ExecuteAsync()
                .GetAwaiter()
                .GetResult();
        });
    }
}
