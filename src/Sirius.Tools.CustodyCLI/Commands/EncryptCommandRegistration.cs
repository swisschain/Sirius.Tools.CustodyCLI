using System.Text.Json;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sirius.Tools.CustodyCLI.Clients;

namespace Sirius.Tools.CustodyCLI.Commands
{

    public class EncryptCommandRegistration : ICommandRegistration
    {
        private readonly CommandFactory _factory;

        public EncryptCommandRegistration(CommandFactory factory)
        {
            _factory = factory;
        }

        public void StartExecution(CommandLineApplication lineApplication)
        {
            lineApplication.Description = "Encrypts custody settings.";
            lineApplication.HelpOption("-?|-h|--help");

            var keyOption = lineApplication.Option(
                "-k|--key <key>",
                "Custody settings public key file",
                CommandOptionType.SingleValue);

            var urlOption = lineApplication.Option(
                "-u|--url <url>",
                "Custody URL",
                CommandOptionType.SingleValue);

            var inOption = lineApplication.Option(
                "-i|--in <in>",
                "Custody settings JSON file",
                CommandOptionType.SingleValue);

            var outOption = lineApplication.Option(
                "-o|--out <in>",
                "Custody encrypted settings JSON file",
                CommandOptionType.SingleValue);

            lineApplication.OnExecute(async () =>
            {
                var keyOptionValue = keyOption.Value();
                var urlOptionValue = urlOption.Value();

                if (string.IsNullOrEmpty(keyOptionValue) && string.IsNullOrEmpty(urlOptionValue))
                    throw new OptionInvalidException(
                        "Either custody settings public key file or custody URL is required.");

                var inOptionValue = inOption.Value();

                if (string.IsNullOrEmpty(inOptionValue))
                    throw new OptionInvalidException("Custody settings JSON file is required.");

                var outOptionValue = outOption.Value();

                if (string.IsNullOrEmpty(outOptionValue))
                    throw new OptionInvalidException("Custody encrypted settings JSON file is required.");

                var command = _factory.CreateCommand(serviceProvider =>
                    new EncryptCommand(
                        keyOptionValue,
                        urlOptionValue,
                        inOptionValue,
                        outOptionValue,
                        serviceProvider.GetRequiredService<JsonSerializerOptions>(),
                        serviceProvider.GetRequiredService<CustodyApiClient>(),
                        serviceProvider.GetRequiredService<ILogger<EncryptCommand>>()));

                return await command.ExecuteAsync();
            });
        }
    }
}
