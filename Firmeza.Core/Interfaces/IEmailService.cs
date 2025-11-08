using System.Threading.Tasks;

namespace Firmeza.Core.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}