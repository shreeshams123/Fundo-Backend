﻿using Models.DTOs;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IUserBL
    {
        Task<string> RegisterUserAsync(RegisterUserDto userdto);
        Task<LoginResponseDto> LoginUserAsync(LoginUserDto userdto);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
       
        Task<bool> ResetPassword(string Token, string Password);

    }
}
