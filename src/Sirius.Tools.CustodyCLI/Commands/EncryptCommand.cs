using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sirius.Tools.CustodyCLI.Contracts;
using Sirius.Tools.CustodyCLI.Crypto;

namespace Sirius.Tools.CustodyCLI.Commands
{
    public class EncryptCommand : ICommand
    {
        private readonly string _keyFile;
        private readonly string _inFile;
        private readonly string _outFile;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<EncryptCommand> _logger;

        public EncryptCommand(
            string keyFile,
            string inFile,
            string outFile,
            JsonSerializerOptions jsonSerializerOptions,
            ILogger<EncryptCommand> logger)
        {
            _keyFile = keyFile;
            _inFile = inFile;
            _outFile = outFile;
            _jsonSerializerOptions = jsonSerializerOptions;
            _logger = logger;
        }

        public async Task<int> ExecuteAsync()
        {
            var inFileContent = await File.ReadAllBytesAsync(_inFile);
            var keyFileContent = await File.ReadAllTextAsync(_keyFile);

            var key = SymmetricEncryptionService.GenerateKey();
            var nonce = SymmetricEncryptionService.GenerateNonce();

            var encryptedSettings = SymmetricEncryptionService.Encrypt(inFileContent, key, nonce);
            var encryptedKey = AsymmetricEncryptionService.Encrypt(key, keyFileContent);

            var message = new EncryptedSettings
            {
                Settings = Convert.ToBase64String(encryptedSettings),
                Key = Convert.ToBase64String(encryptedKey),
                Nonce = Convert.ToBase64String(nonce)
            };

            var json = JsonSerializer.Serialize(message, _jsonSerializerOptions);

            await File.WriteAllTextAsync(_outFile, json, Encoding.UTF8);

            _logger.LogInformation($"File \"{_outFile}\" created.");

            return 0;
        }
    }
}