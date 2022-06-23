using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sirius.Tools.CustodyCLI.Crypto;

namespace Sirius.Tools.CustodyCLI.Commands
{
    public class SignWithdrawalV2Command : ICommand
    {
        private readonly string _privateKeyFile;
        private readonly string _publicKeyFile;
        private readonly string _inFile;
        private readonly string _outFile;
        private readonly ILogger<EncryptCommand> _logger;

        public SignWithdrawalV2Command(string privateKeyFile,
            string publicKeyFile,
            string inFile,
            string outFile,
            ILogger<EncryptCommand> logger)
        {
            _privateKeyFile = privateKeyFile;
            _publicKeyFile = publicKeyFile;
            _inFile = inFile;
            _outFile = outFile;
            _logger = logger;
        }

        public async Task<int> ExecuteAsync()
        {
            var privateKey = await File.ReadAllTextAsync(_privateKeyFile);
            var publicKey = string.IsNullOrWhiteSpace(_publicKeyFile) ? null : await File.ReadAllTextAsync(_publicKeyFile);

            var document = await File.ReadAllTextAsync(_inFile);
            var data = Encoding.UTF8.GetBytes(document);
            
            var signature = AsymmetricEncryptionService.GenerateSignature(data, privateKey);
            bool isValid;
            if (string.IsNullOrWhiteSpace(publicKey))
            {
                _logger.LogInformation("Public key was not specified. The program will not verify generated signature.");
                isValid = true;
            }
            else
            {
                isValid = AsymmetricEncryptionService.VerifySignature(data, signature, publicKey);
            }

            if (!isValid)
            {
                _logger.LogError("Wrong signature");
                await File.WriteAllTextAsync(_outFile, "Wrong signature");
                return 0;
            }
            
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"document\": \"" + document.Replace("\"", "\\\"") + "\",");
            sb.AppendLine("  \"signature\": \"" + Convert.ToBase64String(signature) + "\",");
            sb.AppendLine("}");

            var result = sb.ToString();
            await File.WriteAllTextAsync(_outFile, result);
            return 0;
        }
    }
}
