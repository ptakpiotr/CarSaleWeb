using FluentEmail.Core;
using FluentEmail.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CarSale.Utilities.Mailing
{
    public class FluentEmailSender : IEmailSender
    {
        public FluentEmailSender()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var sender = new SmtpSender(new SmtpClient("smtp.ethereal.email")
            {
                EnableSsl = true,
                Port = 587,
                Credentials = new NetworkCredential(config.GetSection("Mailing:Email").Value, config.GetSection("Mailing:Password").Value)
            }); ;
            Email.DefaultSender = sender;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var em = await Email.From(config.GetSection("Mailing:Email").Value).To(email).Subject(subject)
                .Body(htmlMessage, isHtml: true).SendAsync();
        }
    }
}
