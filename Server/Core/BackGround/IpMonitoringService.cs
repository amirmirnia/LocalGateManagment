using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace ServicesGateManagment.Server.Core.BackGround
{
    public class IpMonitoringService : BackgroundService
    {
        private readonly ILogger<IpMonitoringService> _logger;
        private readonly IApiDataService _IApiDataService;
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, IpStatus> _ipStatuses;
        private int _checkIntervalSeconds;
        private List<MonitoredIp> _monitoredIps;

        public IpMonitoringService(
            ILogger<IpMonitoringService> logger,
            IConfiguration configuration,
            IApiDataService iApiDataService)
        {
            _logger = logger;
            _configuration = configuration;
            _ipStatuses = new Dictionary<string, IpStatus>();
            _monitoredIps = new List<MonitoredIp>();
            _IApiDataService = iApiDataService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("IP Monitoring Service is starting.");

            LoadConfiguration();

            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAllIpsAsync();
                await Task.Delay(TimeSpan.FromSeconds(_checkIntervalSeconds), stoppingToken);
            }

            _logger.LogInformation("IP Monitoring Service is stopping.");
        }

        private void LoadConfiguration()
        {
            _checkIntervalSeconds = _configuration.GetValue<int>("IpMonitoring:CheckIntervalSeconds", 5);

            var ipSection = _configuration.GetSection("IpMonitoring:MonitoredIps");
            if (ipSection.Exists())
            {
                _monitoredIps = ipSection.Get<List<MonitoredIp>>() ?? new List<MonitoredIp>();
            }

            if (!_monitoredIps.Any())
            {
                _monitoredIps = new List<MonitoredIp>
                {
                    new MonitoredIp { Name = "GateAccessManagment", IpAddress = "https://localhost:7124", TimeoutMs = 3000 },
                };
            }

            foreach (var ip in _monitoredIps)
            {
                _ipStatuses[ip.IpAddress] = new IpStatus
                {
                    IpAddress = ip.IpAddress,
                    Name = ip.Name,
                    IsConnected = false,
                    LastChecked = DateTime.UtcNow
                };
            }

            _logger.LogInformation($"Monitoring {_monitoredIps.Count} IP addresses with {_checkIntervalSeconds} seconds interval.");
        }

        private async Task CheckAllIpsAsync()
        {
            var tasks = _monitoredIps.Select(ip => CheckIpStatusAsync(ip));
            await Task.WhenAll(tasks);
        }

        private async Task CheckIpStatusAsync(MonitoredIp monitoredIp)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(monitoredIp.IpAddress, monitoredIp.TimeoutMs);

                var previousStatus = _ipStatuses[monitoredIp.IpAddress];
                var isConnected = reply.Status == IPStatus.Success;

                _ipStatuses[monitoredIp.IpAddress] = new IpStatus
                {
                    IpAddress = monitoredIp.IpAddress,
                    Name = monitoredIp.Name,
                    IsConnected = isConnected,
                    LastChecked = DateTime.UtcNow,
                    ResponseTime = isConnected ? reply.RoundtripTime : null,
                    LastStatusChange = previousStatus.IsConnected != isConnected
                        ? DateTime.UtcNow
                        : previousStatus.LastStatusChange
                };

                if (previousStatus.IsConnected != isConnected)
                {
                    var statusText = isConnected ? "CONNECTED" : "DISCONNECTED";
                    _logger.LogWarning($"IP Status Changed: {monitoredIp.Name} ({monitoredIp.IpAddress}) is now {statusText}");

                    // Execute function when connection status changes
                    if (isConnected)
                    {
                        await OnIpConnected(monitoredIp);
                    }
                    else
                    {
                        await OnIpDisconnected(monitoredIp);
                    }
                }
                else if (isConnected)
                {
                    _logger.LogDebug($"IP Check: {monitoredIp.Name} ({monitoredIp.IpAddress}) is connected. Response time: {reply.RoundtripTime}ms");
                }
                else
                {
                    _logger.LogDebug($"IP Check: {monitoredIp.Name} ({monitoredIp.IpAddress}) is disconnected.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking IP {monitoredIp.Name} ({monitoredIp.IpAddress})");

                _ipStatuses[monitoredIp.IpAddress] = new IpStatus
                {
                    IpAddress = monitoredIp.IpAddress,
                    Name = monitoredIp.Name,
                    IsConnected = false,
                    LastChecked = DateTime.UtcNow,
                    ErrorMessage = ex.Message
                };
            }
        }

        public IpStatus GetIpStatus(string ipAddress)
        {
            return _ipStatuses.TryGetValue(ipAddress, out var status)
                ? status
                : new IpStatus { IpAddress = ipAddress, IsConnected = false };
        }

        public IEnumerable<IpStatus> GetAllStatuses()
        {
            return _ipStatuses.Values.ToList();
        }

        private async Task OnIpConnected(MonitoredIp monitoredIp)
        {
            try
            {
                _logger.LogInformation($"Executing OnIpConnected for {monitoredIp.Name} ({monitoredIp.IpAddress})");

                // Example function: You can replace this with your custom logic
                // For example: Send notification, update database, trigger other services, etc.

               await _IApiDataService.PostDataInDBLocalToExternal();

                // Example 1: Log the connection event
                await LogConnectionEvent(monitoredIp, true);

                // Example 2: Send notification (implement your notification logic)
                await SendConnectionNotification(monitoredIp, true);

                // Example 3: Execute custom action based on specific IP
                if (monitoredIp.IpAddress == "192.168.1.100")
                {
                    await HandleDatabaseServerConnected();
                }

                _logger.LogInformation($"OnIpConnected completed for {monitoredIp.Name}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in OnIpConnected for {monitoredIp.Name}");
            }
        }

        private async Task OnIpDisconnected(MonitoredIp monitoredIp)
        {
            try
            {
                _logger.LogWarning($"Executing OnIpDisconnected for {monitoredIp.Name} ({monitoredIp.IpAddress})");

                // Example function: You can replace this with your custom logic

                // Example 1: Log the disconnection event
                await LogConnectionEvent(monitoredIp, false);

                // Example 2: Send notification
                await SendConnectionNotification(monitoredIp, false);

                // Example 3: Execute custom action based on specific IP
                if (monitoredIp.IpAddress == "192.168.1.100")
                {
                    await HandleDatabaseServerDisconnected();
                }

                _logger.LogWarning($"OnIpDisconnected completed for {monitoredIp.Name}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in OnIpDisconnected for {monitoredIp.Name}");
            }
        }

        // Example helper methods - Replace with your actual implementation
        private async Task LogConnectionEvent(MonitoredIp ip, bool isConnected)
        {
            var eventType = isConnected ? "CONNECTED" : "DISCONNECTED";
            var logMessage = $"[CONNECTION EVENT] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {ip.Name} ({ip.IpAddress}) - Status: {eventType}";

            // You can write to a file, database, or external logging service
            _logger.LogInformation(logMessage);

            // Example: Write to a file (uncomment if needed)
            // var logPath = Path.Combine("Logs", $"connection_events_{DateTime.UtcNow:yyyyMMdd}.log");
            // await File.AppendAllTextAsync(logPath, logMessage + Environment.NewLine);

            await Task.CompletedTask;
        }

        private async Task SendConnectionNotification(MonitoredIp ip, bool isConnected)
        {
            // Implement your notification logic here
            // For example: Send email, SMS, push notification, webhook, etc.

            _logger.LogInformation($"Notification: {ip.Name} is {(isConnected ? "online" : "offline")}");

            // Example: Call a webhook (uncomment and modify as needed)
            // using var httpClient = new HttpClient();
            // var payload = new {
            //     ipAddress = ip.IpAddress,
            //     name = ip.Name,
            //     status = isConnected ? "connected" : "disconnected",
            //     timestamp = DateTime.UtcNow
            // };
            // await httpClient.PostAsJsonAsync("https://your-webhook-url.com/notify", payload);

            await Task.CompletedTask;
        }

        private async Task HandleDatabaseServerConnected()
        {
            _logger.LogInformation("Database server is back online - executing recovery procedures");

            // Add your custom logic here
            // For example: Reconnect database connections, retry failed operations, etc.

            await Task.CompletedTask;
        }

        private async Task HandleDatabaseServerDisconnected()
        {
            _logger.LogCritical("Database server is offline - executing failover procedures");

            // Add your custom logic here
            // For example: Switch to backup database, cache data locally, alert administrators, etc.

            await Task.CompletedTask;
        }
    }

    public class MonitoredIp
    {
        public string Name { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public int TimeoutMs { get; set; } = 3000;
    }

    public class IpStatus
    {
        public string IpAddress { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsConnected { get; set; }
        public DateTime LastChecked { get; set; }
        public DateTime? LastStatusChange { get; set; }
        public long? ResponseTime { get; set; }
        public string? ErrorMessage { get; set; }
    }
}