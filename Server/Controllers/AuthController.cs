using Microsoft.AspNetCore.Mvc;
using ServicesGateManagment.Server.Core;
using ServicesGateManagment.Shared.Models.ViewModel.User;

namespace ServicesGateManagment.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenService _tokenService;

        public AuthController(JwtTokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto request)
        {
            if (request.Username=="Admin" && request.Password=="Qazwqazw@mir01")
            {
                var token = _tokenService.GenerateToken(request.Username);
                return Ok(new { Token = token });
            }



            return Unauthorized();
        }
    }

}
