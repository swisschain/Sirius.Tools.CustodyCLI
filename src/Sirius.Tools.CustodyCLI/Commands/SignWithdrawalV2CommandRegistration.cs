using System.Text.Json;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sirius.Tools.CustodyCLI.Commands
{
    public class SignWithdrawalV2CommandRegistration : ICommandRegistration
    {
        private readonly CommandFactory _factory;

        public SignWithdrawalV2CommandRegistration(CommandFactory factory)
        {
            _factory = factory;
        }

        public void StartExecution(CommandLineApplication lineApplication)
        {
            lineApplication.Description = "Signs withdrawal v2 request.";
            lineApplication.HelpOption("-?|-h|--help");

            var privateKeyOption = lineApplication.Option(
                "-priv|--private-key <key>",
                "Path to private key to generate signature with",
                CommandOptionType.SingleValue);
            
            var publicKeyOption = lineApplication.Option(
                "-pub|--public-key <key>",
                "Path to public key to verify generated signature with",
                CommandOptionType.SingleValue);

            var inOption = lineApplication.Option(
                "-i|--in <in>",
                "Path to text file containing withdrawal v2 request",
                CommandOptionType.SingleValue);

            var outOption = lineApplication.Option(
                "-o|--out <in>",
                "Output file with generated signature",
                CommandOptionType.SingleValue);

            lineApplication.OnExecute(async () =>
            {
                var privateKeyOptionValue = privateKeyOption.Value();
                if (string.IsNullOrEmpty(privateKeyOptionValue))
                    throw new OptionInvalidException(
                        "Cannot generate signature without private key.");
                
                var inOptionValue = inOption.Value();

                if (string.IsNullOrEmpty(inOptionValue))
                    throw new OptionInvalidException("Input JSON file is not specified. Cannot generate signature.");

                var outOptionValue = outOption.Value();
                if (string.IsNullOrEmpty(outOptionValue))
                    throw new OptionInvalidException("Output file for generated signature is not specified. Aborting.");

                var publicKeyOptionValue = publicKeyOption.Value();

                var command = _factory.CreateCommand(serviceProvider =>
                    new SignWithdrawalV2Command(
                        privateKeyOptionValue,
                        publicKeyOptionValue,
                        inOptionValue,
                        outOptionValue,
                        serviceProvider.GetRequiredService<ILogger<EncryptCommand>>()));

                return await command.ExecuteAsync();
            });
        }
    }
}
