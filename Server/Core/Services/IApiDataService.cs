using ServicesGateManagment.Shared.Models.Common;

namespace ServicesGateManagment.Server;

public interface IApiDataService
{
    Task<string> FetchDataAsync(string endpoint);
    Task<byte[]> FetchDataAsBytesAsync(string endpoint);
    Task<string> PostDataAsync(string endpoint, FetchDataRequest FetchDataRequest);

    Task PostDataInDBLocalToExternal();
}