using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string bodyHtml)
    {
        var smtpServer = _configuration["EmailSettings:SmtpServer"];
        var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
        var senderName = _configuration["EmailSettings:SenderName"];
        var senderEmail = _configuration["EmailSettings:SenderEmail"];
        var senderPassword = _configuration["EmailSettings:SenderPassword"];

        var message = new MailMessage();
        message.From = new MailAddress(senderEmail, senderName);
        message.To.Add(new MailAddress(toEmail));
        message.Subject = subject;
        message.Body = bodyHtml;
        message.IsBodyHtml = true;

        using (var client = new SmtpClient(smtpServer, smtpPort))
        {
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(senderEmail, senderPassword);
            await client.SendMailAsync(message);
        }
    }
}
