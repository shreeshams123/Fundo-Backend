using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Utilities
{
    public class PasswordHelper
    {
        public static string GenerateHashedPassword(string Password)
        {
            return BCrypt.Net.BCrypt.HashPassword(Password);
        }
        public static bool VerifyPassword(string Password,string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(Password, hashedPassword);
        }
    }
}
