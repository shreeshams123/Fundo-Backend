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
        public async Task  RegisterUserAsync(User user)
        {
            
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

        }
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }
       

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u=>u.Email == email);  
        }

       public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            
            await _context.SaveChangesAsync();
        }
        
    }
}
