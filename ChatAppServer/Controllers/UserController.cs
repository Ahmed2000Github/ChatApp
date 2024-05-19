using ChatAppCore.DTOs;
using ChatAppServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatAppServer.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }


        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginCredentialsDTO parameters)
        {
            var (response, error) = await _userServices.Loging(parameters);
            if (response is null)
            {
            return BadRequest(error);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] UserDTO parameters)
        {
            var (response, error) = await _userServices.Register(parameters);
            if (response is null)
            {
            return BadRequest(error);
            }
                return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> Refresh([FromBody] string token)
        {
            var (response, error) = await _userServices.RefreshToken(token);
            if (response is null)
            {
                return BadRequest(error);
            }
            return Ok(response);
        }
    }
}
