namespace ServicesGateManagment.Client;

public interface IApiDataService
{
    Task<string> FetchDataAsync(string endpoint);
    Task<byte[]> FetchDataAsBytesAsync(string endpoint);
}