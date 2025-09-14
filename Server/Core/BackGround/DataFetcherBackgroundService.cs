using ServicesGateManagment.Shared;
using ServicesGateManagment.Server;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
namespace Services.BackGround;

public class DataFetcherBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataFetcherBackgroundService> _logger;
    private readonly Dictionary<string, Timer> _activeTimers = new();
    private int _configCheckIntervalSeconds = 30; // Default value

    public DataFetcherBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<DataFetcherBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Data Fetcher Background Service starting...");

        // Load the service configuration on startup
        await LoadServiceConfiguration();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateActiveConfigurations();
                
                // Check for configuration updates based on configured interval
                await Task.Delay(TimeSpan.FromSeconds(_configCheckIntervalSeconds), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in background service");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        _logger.LogInformation("Data Fetcher Background Service stopping...");
    }

    private async Task LoadServiceConfiguration()
    {
        try
        {
            var configFile = Path.Combine(Directory.GetCurrentDirectory(), "Configurations", "service-settings.txt");
            
            if (File.Exists(configFile))
            {
                var lines = await File.ReadAllLinesAsync(configFile);
                foreach (var line in lines)
                {
                    if (line.StartsWith("CheckInterval="))
                    {
                        var value = line.Substring("CheckInterval=".Length).Trim();
                        if (int.TryParse(value, out var interval) && interval > 0)
                        {
                            _configCheckIntervalSeconds = interval;
                            _logger.LogInformation($"Service configuration check interval set to {interval} seconds");
                        }
                    }
                }
            }
            else
            {
                // Create default configuration file
                var defaultConfig = $"# Service Settings{Environment.NewLine}CheckInterval={_configCheckIntervalSeconds}";
                var configDir = Path.GetDirectoryName(configFile);
                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir!);
                }
                await File.WriteAllTextAsync(configFile, defaultConfig);
                _logger.LogInformation($"Created default service configuration with check interval of {_configCheckIntervalSeconds} seconds");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading service configuration, using default interval");
        }
    }

    private async Task UpdateActiveConfigurations()
    {
        using var scope = _serviceProvider.CreateScope();
        var configService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();
        
        var configurations = await configService.LoadConfigurationsAsync();
        
        // Remove timers for disabled or deleted configurations
        var toRemove = _activeTimers.Keys.Where(k => !configurations.Any(c => c.FileName == k && c.IsEnabled)).ToList();
        foreach (var key in toRemove)
        {
            _activeTimers[key]?.Dispose();
            _activeTimers.Remove(key);
            _logger.LogInformation($"Stopped monitoring {key}");
        }
        
        // Add or update timers for enabled configurations
        foreach (var config in configurations.Where(c => c.IsEnabled))
        {
            if (!_activeTimers.ContainsKey(config.FileName))
            {
                var timer = new Timer(
                    async _ => await FetchAndSaveData(config),
                    null,
                    TimeSpan.Zero, // Start immediately
                    TimeSpan.FromMinutes(config.IntervalMinutes)
                );
                
                _activeTimers[config.FileName] = timer;
                _logger.LogInformation($"Started monitoring {config.FileName} every {config.IntervalMinutes} minute(s)");
            }
        }
    }

    private async Task FetchAndSaveData(ApiConfiguration config)
    {
        try
        {
            _logger.LogInformation($"Fetching data for {config.FileName} from {config.ApiEndpoint}");
            
            using var scope = _serviceProvider.CreateScope();
            var apiService = scope.ServiceProvider.GetRequiredService<IApiDataService>();
            var configService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();
            
            // Fetch data from API
            var jsonData = await apiService.FetchDataAsync(config.ApiEndpoint);
            
            // Save to file
            var dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "FetchedData");
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }
            
            var filePath = Path.Combine(dataDirectory, config.FileName);
            
            // Create timestamped backup if file exists
            if (File.Exists(filePath))
            {
                var backupName = Path.GetFileNameWithoutExtension(config.FileName) + 
                                $"_{DateTime.Now:yyyyMMdd_HHmmss}" + 
                                Path.GetExtension(config.FileName);
                var backupPath = Path.Combine(dataDirectory, "Backups", backupName);
                
                var backupDir = Path.GetDirectoryName(backupPath);
                if (!Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir!);
                }
                
                File.Copy(filePath, backupPath, true);
            }
            
            // Write new data
            await File.WriteAllTextAsync(filePath, jsonData);
            
            // Update last fetch time
            config.LastFetchTime = DateTime.Now;
            await configService.UpdateConfigurationAsync(config.FileName, config);
            
            _logger.LogInformation($"Successfully saved data to {config.FileName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching data for {config.FileName}");
        }
    }

    public override void Dispose()
    {
        foreach (var timer in _activeTimers.Values)
        {
            timer?.Dispose();
        }
        _activeTimers.Clear();
        
        base.Dispose();
    }
}