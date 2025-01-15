using System.Threading.Tasks;

public interface IEmailRepository
{
    Task SendEmailAsync(string recipient, string subject, string body);
}
