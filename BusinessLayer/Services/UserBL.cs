﻿using BCrypt.Net;
using BusinessLayer.Interfaces;
using BusinessLayer.Utilities;
using DataLayer.Interfaces;
using DataLayer.Repositories;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models.DTOs;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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
        private readonly ILogger<UserBL> _logger;
        private readonly IRabbitMqService _rabbitMqService;
        public UserBL(IUserDL userRepo, TokenHelper tokenHelper,IConfiguration configuration, IEmailService emailService,ILogger<UserBL> logger,IRabbitMqService rabbitMqService)
        {
            _userRepo = userRepo;
            _tokenHelper = tokenHelper;
            _configuration = configuration;
            _emailService = emailService;
            _logger = logger;
            _rabbitMqService = rabbitMqService;
        }
        public async Task<ApiResponse<string>> RegisterUserAsync(RegisterUserDto userdto)
        {
            _logger.LogInformation("Checking if the user is present");

            if (_userRepo == null)
            {
                _logger.LogError("User repository is not initialized.");
                return new ApiResponse<string> { Success = false, Message = "Internal server error", Data = null };
            }

            var userpresent = await _userRepo.GetUserByEmailAsync(userdto.Email);
            if (userpresent != null)
            {
                _logger.LogWarning("User with {Email} already present", userdto.Email);
                return new ApiResponse<string> { Success = false, Message = "Email already exists", Data = null };
            }

            var pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            if (!Regex.IsMatch(userdto.Password, pattern))
            {
                _logger.LogError("Password does not match the requirements");
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Password should contain minimum 8 characters (at least one special character, one number, one lowercase and one uppercase letter)",
                    Data = null
                };
            }

            _logger.LogInformation("Hashing the password");
            string hashedpassword = PasswordHelper.GenerateHashedPassword(userdto.Password);

            if (string.IsNullOrEmpty(hashedpassword))
            {
                _logger.LogError("Failed to hash the password.");
                return new ApiResponse<string> { Success = false, Message = "Failed to hash password", Data = null };
            }

            var newUser = new User
            {
                Name = userdto.Name,
                Email = userdto.Email,
                Phone = userdto.Phone,
                Password = hashedpassword
            };

            if (newUser == null)
            {
                _logger.LogError("Failed to create a new user. User object is null.");
                return new ApiResponse<string> { Success = false, Message = "User registration failed", Data = null };
            }

            await _userRepo.RegisterUserAsync(newUser);

            if (_rabbitMqService == null)
            {
                _logger.LogError("RabbitMQ service is not initialized.");
                return new ApiResponse<string> { Success = false, Message = "Failed to send email notification", Data = null };
            }

            var emailMessage = new EmailMessageDto
            {
                Email = userdto.Email,
                Subject = "Welcome to FunDo Notes",
                Body = $"<p>Hello {userdto.Name},</p><p>Welcome to FunDo Notes!</p>"
            };

            var messageJson = JsonSerializer.Serialize(emailMessage);
           

            _rabbitMqService.SendMessage(messageJson);

            return new ApiResponse<string> { Success = true, Message = "Registration successful", Data = null };
        }


        public async Task<ApiResponse<LoginResponseDto>> LoginUserAsync(LoginUserDto userdto)
        {
            _logger.LogInformation("Checking if the user present");
            var result= await _userRepo.GetUserByEmailAsync(userdto.Email);

            if (result == null)
            {
                _logger.LogWarning("User does not present");
                return new ApiResponse<LoginResponseDto> {Success=false, Message = "User not found",Data=null };
            }
            _logger.LogInformation("Verifying the password");
            bool isValidPassword= PasswordHelper.VerifyPassword(userdto.Password,result.Password);
            if (isValidPassword)
            {
                var token = _tokenHelper.GenerateJwtToken(result);
                _logger.LogInformation("Password is valid and token is generated");
                var newdto=new LoginResponseDto { Name=result.Name,Email=userdto.Email, Token = token};
                return new ApiResponse<LoginResponseDto> {Success=true, Message="Login Successful",Data=newdto };
            }
            else
            {
                _logger.LogWarning("Invalid password");
                return new ApiResponse<LoginResponseDto> {Success=false, Message = "Invalid Password",Data=null };
            }
        }
        public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            _logger.LogInformation("Checking if the user present");
            var user=await _userRepo.GetUserByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                _logger.LogWarning("User not found");
                return new ApiResponse<string> { Success=false,Message="Email not found",Data=null};
            }
            else
            {
                var resetToken=_tokenHelper.GeneratePasswordResetToken(user);
                _logger.LogInformation("Reset email sent to {Email}", forgotPasswordDto.Email);
                var emailmessage = new EmailMessageDto
                {
                    Email = forgotPasswordDto.Email,
                    Subject = "Token for reset password",
                    Body = resetToken
                };
                await _emailService.SendMailAsync(emailmessage);
                return new ApiResponse<string> { Success=true,Message="Reset link sent to mail",Data=null};
            }
            
        }
        
        public async Task<ApiResponse<string>> ResetPassword(string Token, string Password)
        {
            var userId = _tokenHelper.GetUserIdPasswordResetToken(Token);
            if (userId == null)
            {
                _logger.LogWarning("Invalid token");
                return new ApiResponse<string> { Success=false,Message="Invalid token",Data=null};
            }
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found");
                return new ApiResponse<string> { Success=false,Message="User not found",Data=null};
            }
            var pattern= @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            if(!Regex.IsMatch(Password, pattern))
            {
                _logger.LogWarning("Password mismatch");
                return new ApiResponse<string> { Success = false,Message= "Password should contain minimum 8 characters(atleast one special character,one number,one lowercase and one uppercase letter)",Data=null };
            }
            var hashpassword=PasswordHelper.GenerateHashedPassword(Password);
            user.Password = hashpassword;
            await _userRepo.UpdateUserAsync(user);
            _logger.LogInformation("Password reset successful");
            return new ApiResponse<string> { Success=true,Message="Password reset successful",Data=null};
        }
    }
}
