

using System.Net.Mail;
using System.Net;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Settings;

namespace TaskManager.Application.Services
{
    public  class EmailService: IEmailService
    {
        private readonly MailSettings _mailSettings;
        public EmailService(MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            using var client = new SmtpClient(_mailSettings.SmtpServer, _mailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_mailSettings.Username, _mailSettings.Password),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_mailSettings.SenderEmail, _mailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mail.To.Add(to);

            await client.SendMailAsync(mail);
        }
    }
    
    
}
