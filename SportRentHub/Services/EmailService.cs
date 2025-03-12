using Serilog;
using SportRentHub.Services.Interfaces;
using System.Net.Mail;
using System.Net;

namespace SportRentHub.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmail(string to, string subject, string body)
        {
            try
            {
                // Get SMTP settings from appsettings.json
                var smtpServer = _configuration["SMTP:Server"];
                var smtpPort = int.Parse(_configuration["SMTP:Port"]);
                var smtpEmail = _configuration["SMTP:Email"];
                var smtpPassword = _configuration["SMTP:AppPassword"];

                var credentials = new NetworkCredential(smtpEmail, smtpPassword);

                using (var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = smtpPort,
                    Credentials = credentials,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 5 // 5 giây timeout
                })
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(smtpEmail, "Admin"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true,
                    };
                    mailMessage.To.Add(to);

                    Log.Information($" Đang gửi email đến: {to}, Chủ đề: {subject}");

                    await smtpClient.SendMailAsync(mailMessage);

                    Log.Information(" Email đã gửi thành công!");
                    return true;
                }
            }
            catch (SmtpException smtpEx)
            {
                Log.Error($" Lỗi SMTP khi gửi email: {smtpEx.StatusCode} - {smtpEx.Message}");
            }
            catch (Exception ex)
            {
                Log.Error($" Lỗi không xác định khi gửi email: {ex.Message}");
            }
            return false;
        }
    }
}
