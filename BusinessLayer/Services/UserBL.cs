using BCrypt.Net;
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

        public async Task<string> LoginUserAsync(LoginUserDto userdto)
        {

            var result= await _userRepo.GetUserByEmailAsync(userdto);

            if (result == null)
            {
                return "User not found";
            }
            bool isValidPassword=BCrypt.Net.BCrypt.Verify(userdto.Password,result.Password);
            if (isValidPassword)
            {
                return "Login Successful";
            }
            else
            {
                return "Invalid Password";
            }
        }
    }
}
