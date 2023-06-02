using System.Net;
using System.Text.Json;
using GuardianApiClient.Models;
using Microsoft.Extensions.Logging;

namespace GuardianApiClient;

public class CustodyApiClient
{
    private const string SettingsPath = "/api/settings";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CustodyApiClient> _logger;

    public CustodyApiClient(IHttpClientFactory httpClientFactory, ILogger<CustodyApiClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<SettingsModel?> GetSettingsAsync(string url)
    {
        HttpResponseMessage result;

        try
        {
            result = await _httpClientFactory
                .CreateClient()
                .GetAsync(new Uri(new Uri(url), SettingsPath));
        }
        catch (HttpRequestException exception)
        {
            throw new ApiException(exception.Message, exception);
        }

        await HandleErrorResponseAsync(result);

        var json = await result.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<SettingsModel>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task SetSettingsAsync(string url, string json)
    {
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        HttpResponseMessage result;

        try
        {
            result = await _httpClientFactory
                .CreateClient()
                .PostAsync(new Uri(new Uri(url), SettingsPath), content);
        }
        catch (HttpRequestException exception)
        {
            throw new ApiException(exception.Message, exception);
        }

        await HandleErrorResponseAsync(result);
    }

    private async Task HandleErrorResponseAsync(HttpResponseMessage result)
    {
        if (result.StatusCode == HttpStatusCode.BadRequest)
        {
            var content = await result.Content.ReadAsStringAsync();

            try
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (error != null)
                    throw new ApiException(error.Message);
            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "Can not deserialize error response");
            }
        }

        if (!result.IsSuccessStatusCode)
        {
            throw new ApiException($"{result.StatusCode} {result.ReasonPhrase}");
        }
    }
}
