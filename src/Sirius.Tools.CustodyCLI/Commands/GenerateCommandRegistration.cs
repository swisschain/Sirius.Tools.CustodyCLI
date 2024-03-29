﻿using System.Text.Json;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sirius.Tools.CustodyCLI.Commands
{
    public class GenerateCommandRegistration : ICommandRegistration
    {
        private readonly CommandFactory _factory;

        public GenerateCommandRegistration(CommandFactory factory)
        {
            _factory = factory;
        }

        public void StartExecution(CommandLineApplication lineApplication)
        {
            lineApplication.Description = "Generates keys.";
            lineApplication.HelpOption("-?|-h|--help");

            var typeOption = lineApplication.Option(
                "-t|--type <in>",
                "Key type",
                CommandOptionType.SingleValue);

            var outOption = lineApplication.Option(
                "-o|--out <in>",
                "Key file name without extension",
                CommandOptionType.SingleValue);

            lineApplication.OnExecute(() =>
            {
                var typeOptionValue = typeOption.Value();

                if (string.IsNullOrEmpty(typeOptionValue))
                {
                    typeOptionValue = "rsa";
                }
                else
                {
                    if (typeOptionValue != "rsa" && typeOptionValue != "aes")
                        throw new OptionInvalidException($"Unknown key type {nameof(typeOptionValue)}.");
                }

                var outOptionValue = outOption.Value();

                if (string.IsNullOrEmpty(outOptionValue))
                    throw new OptionInvalidException($"{nameof(outOption)} is required.");

                var command = _factory.CreateCommand(serviceProvider =>
                    new GenerateCommand(
                        typeOptionValue,
                        outOptionValue,
                        serviceProvider.GetRequiredService<JsonSerializerOptions>(),
                        serviceProvider.GetRequiredService<ILogger<GenerateCommand>>()));

                return command.ExecuteAsync()
                    .GetAwaiter()
                    .GetResult();
            });
        }
    }
}
