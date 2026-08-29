using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using UstediPametno.Models;

namespace UstediPametno.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(
            IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task PosaljiAsync(
            string email,
            string naslov,
            string poruka)
        {
            using var porukaEmail = new MailMessage();

            porukaEmail.From = new MailAddress(
                _settings.FromEmail,
                _settings.FromName);

            porukaEmail.To.Add(email);
            porukaEmail.Subject = naslov;
            porukaEmail.Body = poruka;
            porukaEmail.IsBodyHtml = true;

            using var smtp = new SmtpClient(
                _settings.Host,
                _settings.Port);

            smtp.EnableSsl = true;

            smtp.Credentials = new NetworkCredential(
                _settings.Username,
                _settings.Password);

            await smtp.SendMailAsync(porukaEmail);
        }
    }
}