using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Utils;
using TrainingManagementSystem.Api.Common.Configurations;

namespace TrainingManagementSystem.Api.Services.EmailService
{
    public class EmailService(
        IConfiguration _configuration,
        IOptions<EmailSettings> _emailSettings,
        IWebHostEnvironment _env) : IEmailService
    {

        public async Task SendEmailAsync(
            string email,
            string name, 
            string subject, 
            string body) 
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailSettings.Value.SenderName, _emailSettings.Value.SenderEmail));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;


            var builder = new BodyBuilder();

            var logoPath = Path.Combine(_env.WebRootPath, "images", "sitesphil_logo.png");

            var logo = builder.LinkedResources.Add(logoPath);
            logo.ContentId = MimeUtils.GenerateMessageId();

            string formattedBody = $@"
                 <div style='max-width: 600px; margin: auto; font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px; border-radius: 8px; box-shadow: 0px 2px 10px rgba(0, 0, 0, 0.1);'>
                    <div style='background-color: #A82323; color: #ffffff; text-align: center; padding: 15px; border-radius: 8px 8px 0 0;'>
                        <img src='cid:{logo.ContentId}' width='50' height='60' style='vertical-align: middle; margin-right:10px;' />
                        
                        <span style='font-size:22px; font-weight:700; color:white; vertical-align: middle;'>
                            Welcome! Your Account Is Ready
                        </span>
                    </div>
                    <div style='padding: 20px; background-color: #ffffff; border-radius: 0 0 8px 8px;'>
                        <p style='font-size: 16px; margin: 10px 0;'><strong>Sender Name:</strong> {_emailSettings.Value.SenderName}</p>
                        <p style='font-size: 16px; margin: 10px 0;'><strong>Sender Email:</strong> {_emailSettings.Value.SenderEmail}</p>
                        <hr style='border: 1px solid #ddd; margin: 20px 0;'>
                        <p>Your temporary password is provided below.</p>
                        <p><strong>For security, please change your password after logging in.</strong></p>
                        <p style='font-size: 16px; color: #555; line-height: 1.6;'>{body}</p>
                    </div>
                    <div style='text-align: center; padding: 15px; font-size: 12px; color: #777;'>
                        <p style='margin: 0;'>This email was sent automatically. Please do not reply.</p>
                    </div>
                </div>
            ";

            builder.HtmlBody = formattedBody;
            emailMessage.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.Value.SmtpServer, _emailSettings.Value.SmtpPort, false);
            await smtp.AuthenticateAsync(_emailSettings.Value.SmtpUsername, _emailSettings.Value.SmtpPassword);
            await smtp.SendAsync(emailMessage);

            await smtp.DisconnectAsync(true);
        }
    }
}
