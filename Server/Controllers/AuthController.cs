using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServicesGateManagment.Server.Core;
using ServicesGateManagment.Shared.Models.Users;
using ServicesGateManagment.Shared.Models.ViewModel.User;
using System.Threading.Tasks;

namespace ServicesGateManagment.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IUser _User;


        public AuthController(JwtTokenService tokenService, IMapper mapper, IUser user)
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _User = user;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto request)
        {
            if (request.Username == "Admin" && request.Password == "Qazwqazw@mir01")
            {
                var token = _tokenService.GenerateToken(request.Username);
                return Ok(new { Token = token });
            }



            return Unauthorized();
        }

        [HttpPost("RegisterUser")]
        public IActionResult RegisterUser([FromBody] RegisteUserDto RegisteUserDto)
        {

            try
            {
                var finalmodel = _mapper.Map<User>(RegisteUserDto);
                _User.RegisterUser(finalmodel);
                return Ok();
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
        [HttpGet("ListUser")]
        public async Task<IActionResult> ListUser()
        {

            try
            {
                var list = _User.ListUser();
                return Ok(list.Result);
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser(int Id)
        {

            try
            {
                var list =await _User.ListUser();
                return Ok(list);
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
    }

}
