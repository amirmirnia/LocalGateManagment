using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServicesGateManagment.Shared.Models.Common;
using System.Text;
using static System.Net.WebRequestMethods;

namespace ServicesGateManagment.Server;

public class ApiDataService : IApiDataService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiDataService> _logger;

    public ApiDataService(HttpClient httpClient, ILogger<ApiDataService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> FetchDataAsync(string endpoint)
    {
        try
        {
            _logger.LogInformation($"Fetching data from endpoint: {endpoint}");

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync();

            // Format JSON for better readability
            var parsedJson = JToken.Parse(jsonContent);
            var formattedJson = parsedJson.ToString(Formatting.Indented);

            _logger.LogInformation("Data fetched successfully");
            return formattedJson;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"HTTP request failed for endpoint: {endpoint}");
            throw new Exception($"Failed to fetch data from API: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse JSON response");
            throw new Exception($"Invalid JSON response: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred");
            throw;
        }
    }

    public async Task<byte[]> FetchDataAsBytesAsync(string endpoint)
    {
        var jsonData = await FetchDataAsync(endpoint);
        return Encoding.UTF8.GetBytes(jsonData);
    }

    public async Task<string> PostDataAsync(string endpoint, FetchDataRequest FetchDataRequest)
    {

        try
        {

            var responses = await _httpClient.PostAsJsonAsync(endpoint, FetchDataRequest.RequestBody);

            // بررسی وضعیت پاسخ
            if (!responses.IsSuccessStatusCode)
            {
                var errorContent = await responses.Content.ReadAsStringAsync();
                _logger.LogError($"HTTP request failed with status {responses.StatusCode}: {errorContent}");
                throw new HttpRequestException($"Failed to post data to API. Status: {responses.StatusCode}, Content: {errorContent}");
            }
            else
            {
                _logger.LogWarning("API returned empty response");
                return "{}";
            }

        }
        catch (HttpRequestException ex)
        {
            //var errorContent = await ex.HttpResponse?.Content.ReadAsStringAsync() ?? ex.Message;
            //_logger.LogError(ex, $"HTTP POST request failed for endpoint: {endpoint}. Error Content: {errorContent}");
            throw new Exception($"Failed to post data to API: ", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse JSON response");
            throw new Exception($"Invalid JSON response: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred during POST");
            throw;
        }
    }
}