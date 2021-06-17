﻿using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sirius.Tools.CustodyCLI.Contracts;
using Sirius.Tools.CustodyCLI.Crypto;

namespace Sirius.Tools.CustodyCLI.Commands
{
    public class GenerateCommand : ICommand
    {
        private readonly string _outFile;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<GenerateCommand> _logger;

        public GenerateCommand(string outFile,
            JsonSerializerOptions jsonSerializerOptions,
            ILogger<GenerateCommand> logger)
        {
            _outFile = outFile;
            _jsonSerializerOptions = jsonSerializerOptions;
            _logger = logger;
        }

        public async Task<int> ExecuteAsync()
        {
            var keyPair = AsymmetricEncryptionService.GenerateKeyPair();

            var message = new KeyPair
            {
                PrivateKey = AsymmetricEncryptionService.ExportPrivateKey(keyPair.Private),
                PublicKey = AsymmetricEncryptionService.ExportPublicKey(keyPair.Public)
            };

            var json = JsonSerializer.Serialize(message, _jsonSerializerOptions);

            await File.WriteAllTextAsync(_outFile, json, Encoding.UTF8);

            _logger.LogInformation($"File \"{_outFile}\" created.");

            return 0;
        }
    }
}