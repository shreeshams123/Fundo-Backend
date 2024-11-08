using BusinessLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class EmailService:IEmailService
    {
        private  readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration) {
            _configuration = configuration;
           }

        public async Task SendForgotPasswordMailAsync(string email, string resetToken)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("FunDo", _configuration["Email:From"]));
            message.To.Add(new MailboxAddress("",email));
            message.Subject = "Password Reset request";
            message.Body = new TextPart("html")
            {
                Text=$"<p>The token to reset password=>{resetToken}</p>"
            };
            using var smtpClient = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                await smtpClient.ConnectAsync(_configuration["Email:SmtpServer"], int.Parse(_configuration["Email:SmtpPort"]), MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(_configuration["Email:Username"], _configuration["Email:Password"]);
                await smtpClient.SendAsync(message);
            }
            finally
            {
                await smtpClient.DisconnectAsync(true);
            }
        }
    }
}
