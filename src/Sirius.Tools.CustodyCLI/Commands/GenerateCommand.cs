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
    public class GenerateCommand : ICommand
    {
        private readonly string _type;
        private readonly string _outFile;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<GenerateCommand> _logger;

        public GenerateCommand(string type, string outFile,
            JsonSerializerOptions jsonSerializerOptions,
            ILogger<GenerateCommand> logger)
        {
            _type = type;
            _outFile = outFile;
            _jsonSerializerOptions = jsonSerializerOptions;
            _logger = logger;
        }

        public async Task<int> ExecuteAsync()
        {
            if (_type == "rsa")
                await GenerateRsaKey();
            if (_type == "aes")
                await GenerateAesKey();

            return 0;
        }

        private async Task GenerateRsaKey()
        {
            var jsonFileName = $"{_outFile}.json";
            var publicKeyFileName = $"{_outFile}.publickey";
            var privateKeyFileName = $"{_outFile}.privatekey";

            var anyFileExists = false;

            foreach (var file in new[] {jsonFileName, publicKeyFileName, privateKeyFileName})
            {
                if (File.Exists(file))
                {
                    anyFileExists = true;
                    _logger.LogError($"\"{file}\" file already exists.");
                }
            }

            if (anyFileExists)
                return;

            var keyPair = AsymmetricEncryptionService.GenerateKeyPair();

            var privateKey = AsymmetricEncryptionService.ExportPrivateKey(keyPair.Private);
            var publicKey = AsymmetricEncryptionService.ExportPublicKey(keyPair.Public);

            var message = new KeyPair {PrivateKey = privateKey, PublicKey = publicKey};

            var json = JsonSerializer.Serialize(message, _jsonSerializerOptions);

            await File.WriteAllTextAsync(jsonFileName, json, Encoding.UTF8);
            _logger.LogInformation($"File \"{jsonFileName}\" created.");

            await File.WriteAllTextAsync(publicKeyFileName, publicKey, Encoding.UTF8);
            _logger.LogInformation($"File \"{publicKeyFileName}\" created.");

            await File.WriteAllTextAsync(privateKeyFileName, privateKey, Encoding.UTF8);
            _logger.LogInformation($"File \"{privateKeyFileName}\" created.");
        }

        private async Task GenerateAesKey()
        {
            var jsonFileName = $"{_outFile}.key";

            if (File.Exists(jsonFileName))
            {
                _logger.LogError($"\"{jsonFileName}\" file already exists.");
                return;
            }

            var key = SymmetricEncryptionService.GenerateKey();
            var nonce = SymmetricEncryptionService.GenerateNonce();

            var message = new SymmetricKey
            {
                Secret = Convert.ToBase64String(key), Nonce = Convert.ToBase64String(nonce),
            };

            var json = JsonSerializer.Serialize(message, _jsonSerializerOptions);

            await File.WriteAllTextAsync(jsonFileName, json, Encoding.UTF8);

            _logger.LogInformation($"File \"{jsonFileName}\" created.");
        }
    }
}
