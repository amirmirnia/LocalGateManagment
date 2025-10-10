using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var user =await _User.GetUser(request);
            if (user!=null)
            {
                var token = _tokenService.GenerateToken(user.Email,user.Role.ToString(),user.FirstName);
                return Ok(new { Token = token });
            }

            return Unauthorized();




        }
        [Authorize]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "You are authorized!" });
        }
        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto ChangePasswordDto)
        {

            try
            {
               await _User.ChangePassword(ChangePasswordDto);
                return Ok(true);
            }
            catch (Exception)
            {

                return BadRequest();
            }
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
        [HttpGet("GetUser/{id}")]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {

            try
            {
                var user =await _User.GetUserById(id);
                return Ok(user);
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
        [HttpDelete("DeleteUser/{Id}")]
        public async Task<IActionResult> DeleteUser(int Id)
        {

            try
            {
                var user = await _User.DeleteUser(Id);
                return Ok(user);
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUser)
        {

            try
            {
                var finalmodel = _mapper.Map<User>(updateUser);
                var user = await _User.UpdateUser(finalmodel);
                return Ok(user);
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
    }

}
