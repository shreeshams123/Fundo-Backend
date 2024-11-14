using DataLayer.Data;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.DTOs;
using Models.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public class UserDL : IUserDL
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserDL> _logger;

        public UserDL(ApplicationDbContext context, ILogger<UserDL> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task RegisterUserAsync(User user)
        {
            try
            {
                _logger.LogInformation("Attempting to add a new user with Email: {Email}", user.Email);
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User with Email: {Email} successfully registered.", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering user with Email: {Email}", user.Email);
                throw; 
            }
        }
        public async Task<User> GetUserByIdAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve user with ID: {UserId}", userId);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    _logger.LogWarning("User with ID: {UserId} not found.", userId);
                }
                else
                {
                    _logger.LogInformation("User with ID: {UserId} retrieved successfully.", userId);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user with ID: {UserId}", userId);
                throw;  
            }
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve user with Email: {Email}", email);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    _logger.LogWarning("User with Email: {Email} not found.", email);
                }
                else
                {
                    _logger.LogInformation("User with Email: {Email} retrieved successfully.", email);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user with Email: {Email}", email);
                throw;  
            }
        }
        public async Task UpdateUserAsync(User user)
        {
            try
            {
                _logger.LogInformation("Attempting to update user with ID: {UserId}", user.Id);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User with ID: {UserId} successfully updated.", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user with ID: {UserId}", user.Id);
                throw;
            }
        }
    }
}
