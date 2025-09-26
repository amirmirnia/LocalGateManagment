using System.Net.Http.Json;
using ServicesGateManagment.Shared;
using ServicesGateManagment.Shared.Models.Common;
using ServicesGateManagment.Shared.Models.ViewModel.Vehicles;

namespace ServicesGateManagment.Client.Services;

public class VehicleService: IVehicleService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<VehicleService> _logger;

    public VehicleService(HttpClient httpClient, ILogger<VehicleService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<int> CountVehicleInFileJson()
    {

        try
        {
            var response = await _httpClient.GetAsync("api/VehicleInquire/CountVehicleInFileJson");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<int>();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading configurations from API");
            throw;
        }
    }

    public async Task<VehicleInquireResultVm> CreateVehicleInquire(CreateVehicleInquireRequest model)
    {
        try{
            var response = await _httpClient.PostAsJsonAsync("api/VehicleInquire/inquire", model);
            response.EnsureSuccessStatusCode();

            var configurations = await response.Content.ReadFromJsonAsync<VehicleInquireResultVm>();
            return configurations ?? null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading configurations from API");
            throw;
        }
    }

    public async Task<VehicleInquireResultVm> CreateVehicleInquireApi(string EndPoint,CreateVehicleInquireRequest model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/VehicleInquire/inquireApi", model);

            var configurations = await response.Content.ReadFromJsonAsync<VehicleInquireResultVm>();
            return configurations ?? null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading configurations from API");
            throw;
        }
    }

    public async Task<List<VehicleInquireRequestJsonVM>> GetAllRequestVehicle()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/VehicleInquire/GetAllRequestVehicle");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<VehicleInquireRequestJsonVM>>();
            return result ?? null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading configurations from API");
            throw;
        }
    }
}
