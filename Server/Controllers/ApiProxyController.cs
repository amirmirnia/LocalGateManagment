using Microsoft.AspNetCore.Mvc;
using ServicesGateManagment.Server;
using ServicesGateManagment.Shared;
using ServicesGateManagment.Shared.Models.Common;
using static System.Net.WebRequestMethods;

namespace ServicesGateManagment.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiProxyController : ControllerBase
{
    private readonly IApiDataService _apiDataService;
    private readonly ILogger<ApiProxyController> _logger;
    private readonly string _targetUrl; // URL مورد نظر


    public ApiProxyController(IApiDataService apiDataService, ILogger<ApiProxyController> logger,IConfiguration configuration)
    {
        _apiDataService = apiDataService;
        _logger = logger;
        _targetUrl = configuration.GetSection("ApiSettings:TargetUrl").Value;

    }

    [HttpPost("fetch")]
    public async Task<IActionResult> FetchData([FromBody] FetchDataRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Endpoint))
            {
                return BadRequest("Endpoint is required");
            }

            _logger.LogInformation($"Proxying request to: {request.Endpoint}");

            // اگر RequestBody وجود داشته باشه، از متد POST استفاده می‌کنیم
            string data;
            if (request.RequestBody != null)
            {
                data = await _apiDataService.PostDataAsync(request.Endpoint, request);
            }
            else
            {
                data = await _apiDataService.FetchDataAsync(request.Endpoint);
            }

            return Ok(new FetchDataResponse
            {
                Data = data,
                Success = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching data from: {request.Endpoint}");
            return StatusCode(500, new FetchDataResponse
            {
                Success = false,
                Error = ex.Message
            });
        }

    }


    [HttpGet]
    public async Task<bool> CheckConnection()
    {
        try
        {
            return await _apiDataService.CheckConnection(_targetUrl);
        }
        catch
        {
            return false;
        }
    }
}

