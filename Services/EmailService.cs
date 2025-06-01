using GarageMasterBE.Models;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GarageMasterBE.Services
{
    public class EmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value ?? throw new ArgumentNullException(nameof(smtpSettings));
            
            if (string.IsNullOrWhiteSpace(_smtpSettings.FromEmail))
                throw new ArgumentException("SMTP FromEmail cannot be null or empty.", nameof(_smtpSettings.FromEmail));
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Email address cannot be null or empty.", nameof(toEmail));

            try
            {
                using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password),
                    EnableSsl = _smtpSettings.EnableSsl
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.FromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // TODO: Log exception here if you have a logging service
                // _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw new InvalidOperationException($"Gửi email đến {toEmail} thất bại.", ex);
            }
        }

        public async Task SendConfirmationEmailAsync(string toEmail, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Confirmation code cannot be null or empty.", nameof(code));

            string subject = "GarageMaster - Mã xác thực email của bạn";
            string body = $@"
                <p>Chào bạn,</p>
                <p>Mã xác thực để kích hoạt tài khoản GarageMaster của bạn là:</p>
                <h2>{code}</h2>
                <p>Mã này có hiệu lực trong 10 phút.</p>
                <p>Trân trọng,</p>
                <p>GarageMaster Team</p>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}
