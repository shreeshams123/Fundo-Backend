using Models.DTOs;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface IUserDL
    {
        Task<bool> IsUserPresent(string email);
        Task RegisterUserAsync(User user);
        Task<User> GetUserByEmailAsync(LoginUserDto userdto);
        
    }
}
