using DataLayer.Data;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.DTOs;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public class UserDL : IUserDL
    {
        private readonly ApplicationDbContext _context;
        public UserDL(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task  AddUserAsync(RegisterUserDto userdto)
        {
            var user = new User {
            Name=userdto.Name,
            Email=userdto.Email,
            Password=userdto.Password,
            Phone=userdto.Phone,
            ConfirmPassword=userdto.ConfirmPassword,
            };
             await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUserRegistered(string email)
        {
            return await _context.Users.AnyAsync(u=>u.Email==email);
        }
    }
}
