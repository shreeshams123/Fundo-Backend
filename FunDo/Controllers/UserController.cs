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
        [HttpPost("user")]
        
        public async Task<IActionResult> RegisterUser(RegisterUserDto userdto)
        {
            var result= await _userBL.RegisterUserAsync(userdto); 
            if(result== "User with this email already exists")
                return BadRequest(new {Message=result});
            return Ok(result);
        }
    }
}
