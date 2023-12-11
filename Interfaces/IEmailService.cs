namespace FronyToBack.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAysnc(string emailTo, string subject, string body, bool isHtml = false);
    }
}
