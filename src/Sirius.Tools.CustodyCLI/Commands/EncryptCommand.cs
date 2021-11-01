using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sirius.Tools.CustodyCLI.Clients;
using Sirius.Tools.CustodyCLI.Clients.Models;
using Sirius.Tools.CustodyCLI.Contracts;
using Sirius.Tools.CustodyCLI.Crypto;

namespace Sirius.Tools.CustodyCLI.Commands
{
    public class EncryptCommand : ICommand
    {
        private readonly string _keyFile;
        private readonly string _url;
        private readonly string _inFile;
        private readonly string _outFile;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly CustodyApiClient _custodyApiClient;
        private readonly ILogger<EncryptCommand> _logger;

        public EncryptCommand(
            string keyFile,
            string url,
            string inFile,
            string outFile,
            JsonSerializerOptions jsonSerializerOptions,
            CustodyApiClient custodyApiClient,
            ILogger<EncryptCommand> logger)
        {
            _keyFile = keyFile;
            _url = url;
            _inFile = inFile;
            _outFile = outFile;
            _jsonSerializerOptions = jsonSerializerOptions;
            _custodyApiClient = custodyApiClient;
            _logger = logger;
        }

        public async Task<int> ExecuteAsync()
        {
            if (!string.IsNullOrEmpty(_keyFile) && !File.Exists(_keyFile))
            {
                _logger.LogError($"\"{_keyFile}\" file not found.");
                return 0;
            }

            if (!File.Exists(_inFile))
            {
                _logger.LogError($"\"{_inFile}\" file not found.");
                return 0;
            }

            if (File.Exists(_outFile))
            {
                _logger.LogError($"\"{_outFile}\" file already exists.");
                return 0;
            }

            string keyFileContent;

            if (!string.IsNullOrEmpty(_keyFile))
            {
                keyFileContent = await File.ReadAllTextAsync(_keyFile);
                keyFileContent = keyFileContent.Replace(@"\r\n", Environment.NewLine);
            }
            else
            {
                SettingsModel settings;

                try
                {
                    settings = await _custodyApiClient.GetSettingsAsync(_url);
                }
                catch (ApiException exception)
                {
                    _logger.LogError(exception, exception.Message);

                    return 0;
                }

                keyFileContent = settings.Keys.Settings;
            }

            var inFileContent = await File.ReadAllBytesAsync(_inFile);

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
