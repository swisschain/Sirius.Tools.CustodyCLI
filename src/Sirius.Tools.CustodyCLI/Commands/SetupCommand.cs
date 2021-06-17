using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Sirius.Tools.CustodyCLI.Commands
{
    public class SetupCommand : ICommand
    {
        private readonly string _url;
        private readonly string _file;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SetupCommand> _logger;

        public SetupCommand(
            string url,
            string file,
            IHttpClientFactory httpClientFactory,
            ILogger<SetupCommand> logger)
        {
            _url = url;
            _file = file;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<int> ExecuteAsync()
        {
            var json = await File.ReadAllTextAsync(_file);

            using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var result = _httpClientFactory.CreateClient().PostAsync(_url, content).Result;

            if (!result.IsSuccessStatusCode)
                _logger.LogError($"Can not setup custody: {result.StatusCode} {result.ReasonPhrase}");
            else
                _logger.LogInformation("Settings sent.");

            return 0;
        }
    }
}