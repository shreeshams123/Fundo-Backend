using BusinessLayer.Interfaces;
using DataLayer.Interfaces;
using Models.DTOs;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class UserBL : IUserBL
    {
        private readonly IUserDL _userRepo;
        public UserBL(IUserDL userRepo)
        {
            _userRepo = userRepo;
        }
        public async Task<string> RegisterUserAsync(RegisterUserDto userdto)
        {
            if (await _userRepo.IsUserRegistered(userdto.Email))
            {
                return "User with this email already exists";
            }
            var pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{9,}$";
            if (!Regex.IsMatch(userdto.Password, pattern))
            {
                return "Password should contain minimum 8 characters(atleast one special character,one number,one lowercase and one uppercase letter";
            }
            await _userRepo.AddUserAsync(userdto);
            return "Added user Successfully";
        }
    }
}
