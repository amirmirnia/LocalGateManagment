using ServicesGateManagment.Shared;
using ServicesGateManagment.Shared.Models.Common;

namespace ServicesGateManagment.Client.Services
{
    public interface IConfigService
    {
        Task<List<ApiConfiguration>> LoadConfigurationsAsync();
        Task SaveConfigurationAsync(ApiConfiguration config);
        Task UpdateConfigurationAsync(string fileName, ApiConfiguration config);
        Task DeleteConfigurationAsync(string fileName);
        Task<ApiConfiguration?> GetConfigurationAsync(string fileName);
        Task<string> FetchExternalDataAsync(FetchDataRequest model);
    }
}
