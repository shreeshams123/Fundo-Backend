using BCrypt.Net;
using BusinessLayer.Interfaces;
using DataLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.DTOs;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class UserBL : IUserBL
    {
        private readonly IUserDL _userRepo;
        private readonly IConfiguration _configuration;
        public UserBL(IUserDL userRepo,IConfiguration configuration)
        {
            _userRepo = userRepo;
            _configuration = configuration;
        }
        public async Task<string> RegisterUserAsync(RegisterUserDto userdto)
        {
            if (await _userRepo.IsUserPresent(userdto.Email))
            {
                return "User with this email already exists";
            }
            var pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            if (!Regex.IsMatch(userdto.Password, pattern))
            {
                return "Password should contain minimum 8 characters(atleast one special character,one number,one lowercase and one uppercase letter";
            }
            string hashedpassword=BCrypt.Net.BCrypt.HashPassword(userdto.Password);
            var newUser = new User
            {
                Name = userdto.Name,
                Email = userdto.Email,
                Phone = userdto.Phone,
                Password = hashedpassword
            };
            await _userRepo.RegisterUserAsync(newUser);
            return "Added user Successfully";
        }

        public async Task<LoginResponseDto> LoginUserAsync(LoginUserDto userdto)
        {

            var result= await _userRepo.GetUserByEmailAsync(userdto);

            if (result == null)
            {
                return new LoginResponseDto { Message = "User not found" };
            }
            bool isValidPassword=BCrypt.Net.BCrypt.Verify(userdto.Password,result.Password);
            if (isValidPassword)
            {
                var token = GenerateJwtToken(result);
                return new LoginResponseDto { Message="Login Successful",Token=token };
            }
            else
            {
                return new LoginResponseDto { Message = "Invalid Password" };
            }
        }
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            var creds =new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
            };
            var token = new JwtSecurityToken(
                issuer:_configuration["Jwt:Issuer"],
                audience:_configuration["Jwt:Audience"],
                claims:claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
