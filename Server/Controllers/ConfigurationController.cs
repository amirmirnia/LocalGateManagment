using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesGateManagment.Shared;

namespace ServicesGateManagment.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfigurationService _configurationService;
    private readonly ILogger<ConfigurationController> _logger;

    public ConfigurationController(
        IConfigurationService configurationService,
        ILogger<ConfigurationController> logger)
    {
        _configurationService = configurationService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<ApiConfiguration>>> GetConfigurations()
    {
        try
        {
            var configurations = await _configurationService.LoadConfigurationsAsync();
            return Ok(configurations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading configurations");
            return StatusCode(500, new { Error = "Unable to load configurations" });
        }
    }

    [HttpGet("{fileName}")]
    public async Task<ActionResult<ApiConfiguration>> GetConfiguration(string fileName)
    {
        try
        {
            var configuration = await _configurationService.GetConfigurationAsync(fileName);
            if (configuration == null)
            {
                return NotFound();
            }
            return Ok(configuration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading configuration {FileName}", fileName);
            return StatusCode(500, new { Error = "Unable to load configuration" });
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateConfiguration([FromBody] ApiConfiguration configuration)
    {
        try
        {
            if (configuration == null)
            {
                return BadRequest("Configuration is required");
            }

            if (string.IsNullOrWhiteSpace(configuration.ApiEndpoint))
            {
                return BadRequest("API endpoint is required");
            }

            if (string.IsNullOrWhiteSpace(configuration.FileName))
            {
                return BadRequest("File name is required");
            }

            await _configurationService.SaveConfigurationAsync(configuration);
            return CreatedAtAction(nameof(GetConfiguration), 
                new { fileName = configuration.FileName }, configuration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving configuration");
            return StatusCode(500, new { Error = "Unable to save configuration" });
        }
    }

    [HttpPut("{fileName}")]
    public async Task<ActionResult> UpdateConfiguration(string fileName, [FromBody] ApiConfiguration configuration)
    {
        try
        {
            if (configuration == null)
            {
                return BadRequest("Configuration is required");
            }

            await _configurationService.UpdateConfigurationAsync(fileName, configuration);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating configuration {FileName}", fileName);
            return StatusCode(500, new { Error = "Unable to update configuration" });
        }
    }

    [HttpDelete("{fileName}")]
    public async Task<ActionResult> DeleteConfiguration(string fileName)
    {
        try
        {
            await _configurationService.DeleteConfigurationAsync(fileName);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting configuration {FileName}", fileName);
            return StatusCode(500, new { Error = "Unable to delete configuration" });
        }
    }
}