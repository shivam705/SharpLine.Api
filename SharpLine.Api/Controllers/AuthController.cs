using Azure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SharpLine.Api.Models;
using SharpLine.Api.Models.Dtos;
using SharpLine.Api.Services.IService;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Xml.Serialization;

namespace SharpLine.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;
        protected ResponseDto _response;

        // Simple in-memory token blacklist (for demonstration)
        private static ConcurrentDictionary<string, DateTime> _tokenBlacklist = new ConcurrentDictionary<string, DateTime>();

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config, IAuthService authService)
        {
            _authService = authService;
            _userManager = userManager;
            _config = config;
            _response = new ResponseDto();
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var errorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }
            //await _messageBus.PublishMessage(model.Email, _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));
            return Ok(_response);
        }

        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveRole([FromBody] AssignRoleRequestDto model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Role))
            {
                _response.IsSuccess = false;
                _response.Message = "Email and Role are required";
                return BadRequest(_response);
            }

            var roleName = model.Role.ToUpper();
            var removed = await _authService.RemoveRole(model.Email, roleName);
            if (!removed)
            {
                _response.IsSuccess = false;
                _response.Message = "Failed to remove role";
                return BadRequest(_response);
            }

            _response.Message = "Role removed successfully";
            return Ok(_response);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginResponse = await _authService.Login(model);
            if (loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Username or password is incorrect";
                return BadRequest(_response);
            }
            _response.Result = loginResponse;
            return Ok(_response);
        }



        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequestDto model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Role))
            {
                _response.IsSuccess = false;
                _response.Message = "Email and Role are required";
                return BadRequest(_response);
            }

            var roleName = model.Role.ToUpper();
            var assignRoleSuccessful = await _authService.AssignRole(model.Email, roleName);
            if (!assignRoleSuccessful)
            {
                _response.IsSuccess = false;
                _response.Message = "Failed to assign role";
                return BadRequest(_response);
            }
            _response.Message = "Role assigned successfully";
            return Ok(_response);

        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles([FromQuery] string? email)
        {
            IList<string> roles;
            if (!string.IsNullOrWhiteSpace(email))
            {
                roles = await _authService.GetUserRoles(email);
            }
            else
            {
                // attempt to use authenticated user
                var userName = User?.Identity?.Name;
                if (string.IsNullOrWhiteSpace(userName))
                {
                    _response.IsSuccess = false;
                    _response.Message = "No email provided and no authenticated user";
                    return BadRequest(_response);
                }
                roles = await _authService.GetUserRoles(userName);
            }

            _response.Result = roles;
            return Ok(_response);
        }



        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(token))
            {
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                _tokenBlacklist[token] = jwtToken.ValidTo;
            }


            return Ok(new { message = "User logged out successfully. Token added to blacklist." });
        }


        public static bool IsTokenBlacklisted(string token)
        {
            if (_tokenBlacklist.TryGetValue(token, out var expiry))
            {
                if (DateTime.UtcNow < expiry)
                    return true;
                else
                    _tokenBlacklist.TryRemove(token, out _);
            }
            return false;
        }
    }
    
}
