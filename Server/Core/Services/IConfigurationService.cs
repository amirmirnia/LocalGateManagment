using ServicesGateManagment.Shared;

namespace ServicesGateManagment.Server;

public interface IConfigurationService
{
    Task<List<ApiConfiguration>> LoadConfigurationsAsync();
    Task SaveConfigurationAsync(ApiConfiguration config);
    Task UpdateConfigurationAsync(string fileName, ApiConfiguration config);
    Task DeleteConfigurationAsync(string fileName);
    Task<ApiConfiguration?> GetConfigurationAsync(string fileName);
}