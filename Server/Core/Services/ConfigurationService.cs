using System.Text;
using ServicesGateManagment.Shared;
using Microsoft.Extensions.Logging;

namespace ServicesGateManagment.Server;

public class ConfigurationService : IConfigurationService
{
    private readonly string _configDirectory;
    private readonly string _configFile;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(ILogger<ConfigurationService> logger)
    {
        _logger = logger;
        _configDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Configurations");
        _configFile = Path.Combine(_configDirectory, "api-configs.txt");
        
        // Ensure directory exists
        if (!Directory.Exists(_configDirectory))
        {
            Directory.CreateDirectory(_configDirectory);
        }
    }

    public async Task<List<ApiConfiguration>> LoadConfigurationsAsync()
    {
        var configurations = new List<ApiConfiguration>();
        
        try
        {
            if (!File.Exists(_configFile))
            {
                return configurations;
            }

            var lines = await File.ReadAllLinesAsync(_configFile);
            
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var config = ParseConfigurationLine(line);
                if (config != null)
                {
                    configurations.Add(config);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading configurations");
        }

        return configurations;
    }

    public async Task SaveConfigurationAsync(ApiConfiguration config)
    {
        try
        {
            var configLine = FormatConfigurationLine(config);
            
            // Append to file
            await File.AppendAllTextAsync(_configFile, configLine + Environment.NewLine);
            
            _logger.LogInformation($"Configuration saved for {config.FileName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving configuration");
            throw;
        }
    }

    public async Task UpdateConfigurationAsync(string fileName, ApiConfiguration config)
    {
        try
        {
            var configurations = await LoadConfigurationsAsync();
            var existingConfig = configurations.FirstOrDefault(c => c.FileName == fileName);
            
            if (existingConfig != null)
            {
                configurations.Remove(existingConfig);
            }
            
            configurations.Add(config);
            
            // Rewrite the entire file
            await SaveAllConfigurationsAsync(configurations);
            
            _logger.LogInformation($"Configuration updated for {fileName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating configuration");
            throw;
        }
    }

    public async Task DeleteConfigurationAsync(string fileName)
    {
        try
        {
            var configurations = await LoadConfigurationsAsync();
            configurations.RemoveAll(c => c.FileName == fileName);
            
            await SaveAllConfigurationsAsync(configurations);
            
            _logger.LogInformation($"Configuration deleted for {fileName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting configuration");
            throw;
        }
    }

    public async Task<ApiConfiguration?> GetConfigurationAsync(string fileName)
    {
        var configurations = await LoadConfigurationsAsync();
        return configurations.FirstOrDefault(c => c.FileName == fileName);
    }

    private async Task SaveAllConfigurationsAsync(List<ApiConfiguration> configurations)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# API Configuration File");
        sb.AppendLine("# Format: FileName|ApiEndpoint|IntervalMinutes|IsEnabled|LastFetchTime|CreatedAt");
        sb.AppendLine("#");
        
        foreach (var config in configurations)
        {
            sb.AppendLine(FormatConfigurationLine(config));
        }
        
        await File.WriteAllTextAsync(_configFile, sb.ToString());
    }

    private string FormatConfigurationLine(ApiConfiguration config)
    {
        return $"{config.FileName}|{config.ApiEndpoint}|{config.IntervalMinutes}|{config.IsEnabled}|{config.LastFetchTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""}|{config.CreatedAt:yyyy-MM-dd HH:mm:ss}";
    }

    private ApiConfiguration? ParseConfigurationLine(string line)
    {
        try
        {
            var parts = line.Split('|');
            if (parts.Length < 6)
                return null;

            return new ApiConfiguration
            {
                FileName = parts[0],
                ApiEndpoint = parts[1],
                IntervalMinutes = int.TryParse(parts[2], out var interval) ? interval : 1,
                IsEnabled = bool.TryParse(parts[3], out var enabled) && enabled,
                LastFetchTime = string.IsNullOrEmpty(parts[4]) ? null : DateTime.Parse(parts[4]),
                CreatedAt = DateTime.TryParse(parts[5], out var created) ? created : DateTime.Now
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error parsing configuration line: {line}");
            return null;
        }
    }
}