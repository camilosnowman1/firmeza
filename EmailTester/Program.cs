using System;
using System.Net;
using System.Net.Mail;

namespace EmailTester
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string smtpHost = "smtp.gmail.com";
                int smtpPort = 587;
                string smtpUser = "camilosnow1997@gmail.com";
                string smtpPass = "rrif efjo wvum spvl".Replace(" ", ""); // Remove spaces

                Console.WriteLine($"Testing email to {smtpUser}...");
                Console.WriteLine($"Host: {smtpHost}:{smtpPort}");
                Console.WriteLine($"User: {smtpUser}");
                Console.WriteLine($"Pass (length): {smtpPass.Length}");

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    client.EnableSsl = true;
                    
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(smtpUser),
                        Subject = "Test Email from Firmeza Debugger",
                        Body = "<h1>It works!</h1><p>This is a test email.</p>",
                        IsBodyHtml = true,
                    };
                    mailMessage.To.Add("camilosnow@gmail.com");

                    Console.WriteLine("Sending...");
                    client.Send(mailMessage);
                    Console.WriteLine("SUCCESS: Email sent!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
