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
using Microsoft.Extensions.Logging;
using Models.DTOs;

namespace BusinessLayer.Services
{
    public class EmailService:IEmailService
    {
        private  readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        public EmailService(IConfiguration configuration,ILogger<EmailService> logger) {
            _configuration = configuration;
            _logger = logger;
           }

        public async Task SendMailAsync(EmailMessageDto emailMessageDto)
        {
            _logger.LogInformation("Attempt to create email");
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("FunDo", _configuration["Email:From"]));
            message.To.Add(new MailboxAddress("",emailMessageDto.Email));
            message.Subject = emailMessageDto.Subject;
            message.Body = new TextPart("html")
            {
                Text=$"<p>{emailMessageDto.Body}</p>"
            };
            using var smtpClient = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                _logger.LogInformation("Connecting to smtp server");
                await smtpClient.ConnectAsync(_configuration["Email:SmtpServer"], int.Parse(_configuration["Email:SmtpPort"]), MailKit.Security.SecureSocketOptions.StartTls);
                _logger.LogInformation("Authenticatingwith smtp server");
                await smtpClient.AuthenticateAsync(_configuration["Email:Username"], _configuration["Email:Password"]);
                _logger.LogInformation("Sending mail");
                await smtpClient.SendAsync(message);
                _logger.LogInformation("Sent email to {Email} successfully",emailMessageDto.Email);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,"There was an error sending the mail");
            }
            finally
            {
                _logger.LogInformation("Disconnecting from smtp server");
                await smtpClient.DisconnectAsync(true);
            }
        }
    }
}
