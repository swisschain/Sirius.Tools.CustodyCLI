using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sirius.Tools.CustodyCLI.Clients;

namespace Sirius.Tools.CustodyCLI.Commands
{
    public class SetupCommand : ICommand
    {
        private readonly string _url;
        private readonly string _file;
        private readonly CustodyApiClient _custodyApiClient;
        private readonly ILogger<SetupCommand> _logger;

        public SetupCommand(
            string url,
            string file,
            CustodyApiClient custodyApiClient,
            ILogger<SetupCommand> logger)
        {
            _url = url;
            _file = file;
            _custodyApiClient = custodyApiClient;
            _logger = logger;
        }

        public async Task<int> ExecuteAsync()
        {
            if (!File.Exists(_file))
            {
                _logger.LogError($"\"{_file}\" file not found.");
                return 0;
            }

            var json = await File.ReadAllTextAsync(_file);

            try
            {
                await _custodyApiClient.SetSettingsAsync(_url, json);
            }
            catch (ApiException exception)
            {
                _logger.LogError(exception, exception.Message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while setup custody");
            }

            _logger.LogInformation("Settings successfully sent to custody");
            
            return 0;
        }
    }
}
