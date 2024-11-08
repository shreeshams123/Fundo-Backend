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
        public UserController(IUserBL userBL)
        {
            _userBL = userBL;
        }
        [HttpPost("Register")]

        public async Task<IActionResult> RegisterUser(RegisterUserDto userdto)
        {
            var result = await _userBL.RegisterUserAsync(userdto);
            if (result == "User with this email already exists")
                return BadRequest(new { Message = result });
            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(LoginUserDto userdto)
        {
            var result = await _userBL.LoginUserAsync(userdto);
            if (result.Message == "Login Successful")
            {
                return Ok(result);
            }
            return BadRequest(new { Message = result.Message });

        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            await _userBL.ForgotPasswordAsync(forgotPasswordDto);
            return Ok("A password reset link has been sent to your mail");
        }
        
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            bool isPasswordSet = await _userBL.ResetPassword(resetPasswordDto.Token, resetPasswordDto.Password);
            if (isPasswordSet == false)
            {
                return BadRequest(new { message = "Password couldn't be set" });
            }
            return Ok();
        }
    }
}
