using BCrypt.Net;
using BusinessLayer.Interfaces;
using BusinessLayer.Utilities;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Razor.TagHelpers;
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
        private readonly TokenHelper _tokenHelper;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        public UserBL(IUserDL userRepo, TokenHelper tokenHelper,IConfiguration configuration, IEmailService emailService)
        {
            _userRepo = userRepo;
            _tokenHelper = tokenHelper;
            _configuration = configuration;
            _emailService = emailService;
        }
        public async Task<string> RegisterUserAsync(RegisterUserDto userdto)
        {
            var userpresent = await _userRepo.GetUserByEmailAsync(userdto.Email);
            if (userpresent!=null)
            {
                return "User with this email already exists";
            }
            var pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            if (!Regex.IsMatch(userdto.Password, pattern))
            {
                return "Password should contain minimum 8 characters(atleast one special character,one number,one lowercase and one uppercase letter";
            }
            string hashedpassword= PasswordHelper.GenerateHashedPassword(userdto.Password);
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

            var result= await _userRepo.GetUserByEmailAsync(userdto.Email);

            if (result == null)
            {
                return new LoginResponseDto { Message = "User not found" };
            }
            bool isValidPassword= PasswordHelper.VerifyPassword(userdto.Password,result.Password);
            if (isValidPassword)
            {
                var token = _tokenHelper.GenerateJwtToken(result);
                return new LoginResponseDto { Message="Login Successful",Token=token };
            }
            else
            {
                return new LoginResponseDto { Message = "Invalid Password" };
            }
        }
        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user=await _userRepo.GetUserByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
               
                return false;
            }
            else
            {
                var resetToken=_tokenHelper.GeneratePasswordResetToken(user);
                
                await _emailService.SendForgotPasswordMailAsync(forgotPasswordDto.Email,resetToken);
                return true;
            }
            
        }
        
        public async Task<bool> ResetPassword(string Token, string Password)
        {
            var userId = _tokenHelper.ValidatePasswordResetToken(Token);
            if (userId == null)
            {
                return false;
            }
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            var hashpassword=PasswordHelper.GenerateHashedPassword(Password);
            await _userRepo.UpdateUserAsync(user);
            return true;
        }
    }
}
