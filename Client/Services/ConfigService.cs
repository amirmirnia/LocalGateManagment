using System.Net.Http.Json;
using ServicesGateManagment.Shared;
using ServicesGateManagment.Shared.Models.Common;

namespace ServicesGateManagment.Client.Services;

public class ConfigService: IConfigService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ConfigService> _logger;

    public ConfigService(HttpClient httpClient, ILogger<ConfigService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<ApiConfiguration>> LoadConfigurationsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/configuration");
            response.EnsureSuccessStatusCode();
            
            var configurations = await response.Content.ReadFromJsonAsync<List<ApiConfiguration>>();
            return configurations ?? new List<ApiConfiguration>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading configurations from API");
            throw;
        }
    }

    public async Task SaveConfigurationAsync(ApiConfiguration config)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/configuration", config);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving configuration to API");
            throw;
        }
    }

    public async Task UpdateConfigurationAsync(string fileName, ApiConfiguration config)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/configuration/{fileName}", config);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating configuration via API");
            throw;
        }
    }

    public async Task DeleteConfigurationAsync(string fileName)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/configuration/{fileName}");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting configuration via API");
            throw;
        }
    }

    public async Task<ApiConfiguration?> GetConfigurationAsync(string fileName)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/configuration/{fileName}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ApiConfiguration>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting configuration from API");
            throw;
        }
    }

    public async Task<string> FetchExternalDataAsync(FetchDataRequest model)
    {
        try
        {
            
            var response = await _httpClient.PostAsJsonAsync("api/ApiProxy/fetch", model);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<FetchDataResponse>();
            
            if (result?.Success != true)
            {
                throw new Exception(result?.Error ?? "Unknown server error");
            }
            
            return result.Data ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching external data via server proxy");
            throw;
        }
    }
}

