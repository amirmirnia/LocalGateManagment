using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ServicesGateManagment.Client.Services;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using ServicesGateManagment.Shared;
using System.Net.Http.Json;

public class ApiDataService : IApiDataService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiDataService> _logger;

    public ApiDataService(IHttpClientFactory httpClientFactory, ILogger<ApiDataService> logger, HttpClient httpClient)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> FetchDataAsync(string endpoint)
    {
        try
        {
            _logger.LogInformation($"Fetching data from endpoint: {endpoint}");
            
            // Create HttpClient - use default for absolute URLs, named for relative URLs
            var httpClient = Uri.IsWellFormedUriString(endpoint, UriKind.Absolute) 
                ? _httpClientFactory.CreateClient()
                : _httpClientFactory.CreateClient("ServicesGateManagment.ServerAPI");
            
            _logger.LogInformation($"Making HTTP GET request to: {endpoint}");
            var response = await httpClient.GetAsync(endpoint);
            
            _logger.LogInformation($"Response status: {response.StatusCode}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"HTTP request failed with status {response.StatusCode}. Response: {errorContent}");
                throw new HttpRequestException($"HTTP {response.StatusCode}: {response.ReasonPhrase}. Response: {errorContent}");
            }
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Response content length: {jsonContent.Length} characters");
            
            // Try to format JSON for better readability, but don't fail if it's not valid JSON
            try
            {
                var parsedJson = JToken.Parse(jsonContent);
                var formattedJson = parsedJson.ToString(Formatting.Indented);
                _logger.LogInformation("Data fetched and formatted successfully");
                return formattedJson;
            }
            catch (JsonException)
            {
                _logger.LogWarning("Response is not valid JSON, returning as-is");
                return jsonContent;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"HTTP request failed for endpoint: {endpoint}. Details: {ex.Message}");
            throw new Exception($"Failed to fetch data from API: {ex.Message}. Check if the endpoint is accessible and CORS is configured.", ex);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, $"Request timeout for endpoint: {endpoint}");
            throw new Exception($"Request timed out: {endpoint}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error occurred while fetching from: {endpoint}");
            throw new Exception($"Unexpected error: {ex.Message}", ex);
        }
    }

    public async Task<byte[]> FetchDataAsBytesAsync(string endpoint)
    {
        var jsonData = await FetchDataAsync(endpoint);
        return System.Text.Encoding.UTF8.GetBytes(jsonData);
    }

    public async Task<bool> CheckConnectionToLiara()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/ApiProxy");
            response.EnsureSuccessStatusCode();

            var configurations = await response.Content.ReadFromJsonAsync<bool>();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading configurations from API");
            throw;
        }
    }

}