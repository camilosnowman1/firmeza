using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Firmeza.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Firmeza.Infrastructure.Services;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try 
        {
            // Get SMTP settings from configuration
            var smtpHost = _configuration["Smtp:Host"];
            var smtpPort = int.Parse(_configuration["Smtp:Port"]);
            var smtpUser = _configuration["Smtp:User"];
            var smtpPass = _configuration["Smtp:Pass"]?.Replace(" ", ""); // Remove spaces from App Password

            Console.WriteLine($"Attempting to send email to {to} using {smtpUser} on {smtpHost}:{smtpPort}");

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
            Console.WriteLine($"Email sent successfully to {to}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CRITICAL EMAIL ERROR: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            throw; // Re-throw to let the controller handle it (or not)
        }
    }
}