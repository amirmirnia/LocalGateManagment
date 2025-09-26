using Microsoft.AspNetCore.Mvc;
using ServicesGateManagment.Shared;
using ServicesGateManagment.Server;
using System.Text.Json;
using ServicesGateManagment.Server.Handlers;
using ServicesGateManagment.Shared.Models.ViewModel.Vehicles;

namespace VehicleDataFetcherBlazor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleInquireController : ControllerBase
{
    private readonly IVehicleInquireService _vehicleInquireService;
    private readonly ILogger<VehicleInquireController> _logger;
    private readonly ApiService _httpClient;

    public VehicleInquireController(
        IVehicleInquireService vehicleInquireService,
        ILogger<VehicleInquireController> logger, ApiService httpClient)
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

    //send to DB Server GateManagment Liara
    /// https://localhost:7012/api/VehicleInquire/inquireApi
    [HttpPost("inquireApi")]
    public async Task<ActionResult<VehicleInquireResultVm>> InquireVehicleAccessApi(CreateVehicleInquireRequest request)
    {
        try
        {
            
            // ارسال درخواست POST
            var response = await _httpClient.PostAsJsonAsync("/Api/Access/inquire/vehicle", request);

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

    [HttpGet("GetAllRequestVehicle")]
    public async Task<ActionResult<List<VehicleInquireRequestJsonVM>>> GetAllRequestVehicle()
    {
        try
        {
            var result = await _vehicleInquireService.GetAllRequestVehicle(); 
            return Ok(result);
        }
        catch (Exception)
        {
            throw; // یا می‌توانید خطا را مدیریت کنید و پاسخ مناسب برگردانید
        }

    }
    [HttpGet("CountVehicleInFileJson")]
    public async Task<ActionResult<int>> CountVehicleInFileJson()
    {
        try
        {
            var result = await _vehicleInquireService.CountVehicleInFileJson("data.json");
            return Ok(result);
        }
        catch (Exception)
        {
            throw; // یا می‌توانید خطا را مدیریت کنید و پاسخ مناسب برگردانید
        }

    }
}