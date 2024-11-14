using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Models.DTOs;
using Models.Entities;

namespace FunDo.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userBL;
        private readonly ILogger<UserController> _logger;   
        public UserController(IUserBL userBL, ILogger<UserController> logger)
        {
            _userBL = userBL;
            _logger = logger;

        }
        [HttpPost("Register")]

        public async Task<IActionResult> RegisterUser(RegisterUserDto userdto)
        {
            _logger.LogInformation("User registration attempt started");
            var apiResponse = await _userBL.RegisterUserAsync(userdto);
            if (apiResponse.Success)
            {
                _logger.LogInformation("Registration successful");
                return Ok(apiResponse);
                
            }
            _logger.LogWarning("Registration failed");
            return BadRequest(apiResponse);

        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(LoginUserDto userdto)
        {
            _logger.LogInformation("Logging user {Email}", userdto.Email);
            var apiresponse = await _userBL.LoginUserAsync(userdto);
            if (apiresponse.Success)
            {
                _logger.LogInformation("Login successful");
                return Ok(apiresponse);
            }
            _logger.LogWarning("Login failed");
            return BadRequest(apiresponse);

        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            _logger.LogInformation("Forgot password started by {Email}",forgotPasswordDto.Email);
            var apiresponse=await _userBL.ForgotPasswordAsync(forgotPasswordDto);
            if (apiresponse.Success)
            {
                _logger.LogInformation("A link has been sent to mail if the user is registered ");
                return Ok(apiresponse);
            }
            return BadRequest(apiresponse);
        }
        
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            _logger.LogInformation("Reset password started");
            var apiresponse= await _userBL.ResetPassword(resetPasswordDto.Token, resetPasswordDto.Password);
            if (apiresponse.Success)
            {
                _logger.LogInformation("Reset password successful");
                return Ok(apiresponse);
            }
            _logger.LogWarning("Reset password failed");
                return BadRequest(apiresponse);
            }
            
    }
}
