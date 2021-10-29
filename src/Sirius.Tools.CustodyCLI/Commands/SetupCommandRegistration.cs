using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sirius.Tools.CustodyCLI.Clients;

namespace Sirius.Tools.CustodyCLI.Commands
{
    public class SetupCommandRegistration : ICommandRegistration
    {
        private readonly CommandFactory _factory;

        public SetupCommandRegistration(CommandFactory factory)
        {
            _factory = factory;
        }

        public void StartExecution(CommandLineApplication lineApplication)
        {
            lineApplication.Description = "Send encrypted settings to custody.";
            lineApplication.HelpOption("-?|-h|--help");

            var urlOption = lineApplication.Option(
                "-u|--url <url>",
                "Custody URL",
                CommandOptionType.SingleValue);

            var fileOption = lineApplication.Option(
                "-f|--file <file>",
                "Custody encrypted settings JSON file",
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
                    new SetupCommand(
                        urlOptionValue,
                        fileOptionValue,
                        serviceProvider.GetRequiredService<CustodyApiClient>(),
                        serviceProvider.GetRequiredService<ILogger<SetupCommand>>()));

                return command.ExecuteAsync()
                    .GetAwaiter()
                    .GetResult();
            });
        }
    }
}
