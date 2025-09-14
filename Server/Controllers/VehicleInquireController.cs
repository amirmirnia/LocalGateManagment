using Microsoft.AspNetCore.Mvc;
using ServicesGateManagment.Shared;
using ServicesGateManagment.Server;
using System.Net.Http;
using System.Reflection;
using ServicesGateManagment.Client.Pages;
using System.Text.Json;

namespace VehicleDataFetcherBlazor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleInquireController : ControllerBase
{
    private readonly IVehicleInquireService _vehicleInquireService;
    private readonly ILogger<VehicleInquireController> _logger;
    private readonly HttpClient _httpClient;

    public VehicleInquireController(
        IVehicleInquireService vehicleInquireService,
        ILogger<VehicleInquireController> logger, HttpClient httpClient)
    {
        _vehicleInquireService = vehicleInquireService;
        _logger = logger;
        _httpClient = httpClient;
    }

    /// https://localhost:7012/api/VehicleInquire/inquire
    [HttpPost("inquire")]
    public async Task<ActionResult<VehicleInquireResultVm>> InquireVehicleAccess(CreateVehicleInquireRequest request)
    {
        try
        {
            _logger.LogInformation($"Received vehicle inquire request for gate: {request.Gate}");

            var result = await _vehicleInquireService.ProcessInquireAsync(request);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request parameters");
            return BadRequest(new { Error = ex.Message });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error communicating with GateAccess API");
            return StatusCode(502, new { Error = "Unable to process request at this time" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing vehicle inquire");
            return StatusCode(500, new { Error = "An unexpected error occurred" });
        }
    }

    [HttpPost("inquireApi")]
    public async Task<ActionResult<VehicleInquireResultVm>> InquireVehicleAccessApi(CreateVehicleInquireRequest request)
    {
        try
        {
            // ارسال درخواست POST
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7124/Api/Access/inquire/vehicle", request);

            // بررسی موفقیت‌آمیز بودن درخواست
            response.EnsureSuccessStatusCode();

            // تبدیل پاسخ به نوع VehicleInquireResultVm
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var result = await response.Content.ReadFromJsonAsync<VehicleInquireResultVm>(options);

            // بررسی null بودن نتیجه
            if (result == null)
            {
                throw new Exception("پاسخ دریافتی از سرور خالی است.");
            }

            return result;
        }
        catch (HttpRequestException ex)
        {
            // مدیریت خطاهای HTTP
            throw new Exception("خطا در ارتباط با سرور: " + ex.Message);
        }
        catch (Exception ex)
        {
            // مدیریت سایر خطاها
            throw new Exception("خطا در پردازش درخواست: " + ex.Message);
        }
    }

    [HttpGet("summary")]
    public async Task<ActionResult<object>> GetInquirySummary()
    {
        try
        {
            var summaryFile = Path.Combine(Directory.GetCurrentDirectory(), "InquireData", "inquire_summary.json");

            if (!System.IO.File.Exists(summaryFile))
            {
                return Ok(new List<object>());
            }

            var json = await System.IO.File.ReadAllTextAsync(summaryFile);
            return Ok(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inquiry summary");
            return StatusCode(500, new { Error = "Unable to retrieve summary" });
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    /// <returns>Service status</returns>
    [HttpGet("health")]
    public ActionResult<object> GetHealth()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.Now,
            Service = "VehicleDataFetcher API"
        });
    }
}