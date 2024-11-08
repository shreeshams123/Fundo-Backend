using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IEmailService
    {
        Task SendForgotPasswordMailAsync(string email,string resetLink);
    }
}
