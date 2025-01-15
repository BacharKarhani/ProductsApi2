using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailRepository : IEmailRepository
{
    private readonly string _smtpHost = "smtp.gmail.com";
    private readonly int _smtpPort = 587;
    private readonly string _senderEmail = "bacharkarhani0@gmail.com";
    private readonly string _senderPassword = "rvzkpeemtmxmfdrl"; 

    public async Task SendEmailAsync(string recipient, string subject, string body)
    {
        try
        {
            var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_senderEmail, _senderPassword),
                EnableSsl = true
            };

            var message = new MailMessage(_senderEmail, recipient, subject, body)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email sending failed: {ex.Message}");
            throw new Exception("Email sending failed.", ex);
        }
    }
}
