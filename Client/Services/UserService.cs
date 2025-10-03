using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ServicesGateManagment.Shared;
using ServicesGateManagment.Shared.Models.Common;
using ServicesGateManagment.Shared.Models.ViewModel.User;
using ServicesGateManagment.Shared.Models.ViewModel.Vehicles;
using static System.Net.WebRequestMethods;

namespace ServicesGateManagment.Client.Services;

public class UserService : IUser
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<VehicleService> _logger;

    public UserService(HttpClient httpClient, ILogger<VehicleService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> Login(LoginDto model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", model);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<LoginResultDto>();
            return result.Token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading configurations from API");
            throw;
        }
    }
}
