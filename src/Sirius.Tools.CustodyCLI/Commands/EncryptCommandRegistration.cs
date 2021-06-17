﻿using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

                if (string.IsNullOrEmpty(keyOptionValue))
                    throw new OptionInvalidException($"{nameof(keyOption)} is required.");

                if (!File.Exists(keyOptionValue))
                    throw new OptionInvalidException($"\"{keyOptionValue}\" file not found.");

                var inOptionValue = inOption.Value();

                if (string.IsNullOrEmpty(inOptionValue))
                    throw new OptionInvalidException($"{nameof(inOption)} is required.");

                if (!File.Exists(inOptionValue))
                    throw new OptionInvalidException($"\"{inOptionValue}\" file not found.");

                var outOptionValue = outOption.Value();

                if (string.IsNullOrEmpty(outOptionValue))
                    throw new OptionInvalidException($"{nameof(outOption)} is required.");

                if (File.Exists(outOptionValue))
                    throw new OptionInvalidException($"\"{outOptionValue}\" file already exists.");

                var command = _factory.CreateCommand(serviceProvider =>
                    new EncryptCommand(
                        keyOptionValue,
                        inOptionValue,
                        outOptionValue,
                        serviceProvider.GetRequiredService<JsonSerializerOptions>(),
                        serviceProvider.GetRequiredService<ILogger<EncryptCommand>>()));

                return await command.ExecuteAsync();
            });
        }
    }
}