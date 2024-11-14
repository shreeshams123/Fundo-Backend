using Models;
using Models.DTOs;
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
        Task<ApiResponse<string>> RegisterUserAsync(RegisterUserDto userdto);
        Task<ApiResponse<LoginResponseDto>> LoginUserAsync(LoginUserDto userdto);
        Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);


        Task<ApiResponse<string>> ResetPassword(string Token, string Password);

    }
}
