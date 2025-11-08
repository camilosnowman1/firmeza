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
        // Get SMTP settings from configuration (we will use User Secrets for this)
        var smtpHost = _configuration["Smtp:Host"];
        var smtpPort = int.Parse(_configuration["Smtp:Port"]);
        var smtpUser = _configuration["Smtp:User"];
        var smtpPass = _configuration["Smtp:Pass"];

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
    }
}