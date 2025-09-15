using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServicesGateManagment.Server;

namespace YourProject.Services
{
    public class NetworkCheckBackgroundService : BackgroundService
    {
        private readonly ILogger<NetworkCheckBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly HttpClient _httpClient;
        private readonly string _targetUrl = "https://localhost:7124/"; // URL مورد نظر
        private readonly int _timeoutSeconds = 5; // تایم اوت درخواست (5 ثانیه)

        public NetworkCheckBackgroundService(
            ILogger<NetworkCheckBackgroundService> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;

            // HttpClient با تنظیمات مناسب برای localhost
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(_timeoutSeconds);

            // برای HTTPS localhost با گواهی خودامضا
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            _httpClient = new HttpClient(handler);
            _httpClient.Timeout = TimeSpan.FromSeconds(_timeoutSeconds);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Network Check Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    bool isConnected = await CheckUrlConnectivity();

                    if (isConnected)
                    {
                        _logger.LogInformation($"Connection to {_targetUrl} is available. Executing data sync...");

                        using var scope = _serviceScopeFactory.CreateScope();
                        var apiDataService = scope.ServiceProvider.GetRequiredService<IApiDataService>();

                        await apiDataService.PostDataInDBLocalToExternal();

                        _logger.LogInformation("Data sync completed successfully");
                    }
                    else
                    {
                        _logger.LogWarning($"No connection to {_targetUrl}. Skipping data sync...");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during network check or data sync");
                }

                // صبر 2 ثانیه تا چک بعدی
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }

            _logger.LogInformation("Network Check Background Service stopped");
        }

        private async Task<bool> CheckUrlConnectivity()
        {
            try
            {
                using var response = await _httpClient.GetAsync(_targetUrl);

                // اگر پاسخ موفقیت‌آمیز باشد (2xx status codes)
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning($"HTTP request failed for {_targetUrl}: {ex.Message}");
                return false;
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                _logger.LogWarning($"Request timeout for {_targetUrl}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to connect to {_targetUrl}");
                return false;
            }
        }

        public override void Dispose()
        {
            _httpClient?.Dispose();
            base.Dispose();
        }
    }


}

