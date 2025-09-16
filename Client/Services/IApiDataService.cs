namespace ServicesGateManagment.Client.Services;

public interface IApiDataService
{
    Task<string> FetchDataAsync(string endpoint);
    Task<byte[]> FetchDataAsBytesAsync(string endpoint);
}