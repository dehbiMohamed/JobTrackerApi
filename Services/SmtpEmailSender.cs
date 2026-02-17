using JobTracker.api.Config;
using JobTracker.api.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace JobTracker.api.Services;

public class SmtpEmailSender(IOptions<EmailOptions> opt) : IEmailSender
{
    private readonly EmailOptions _o = opt.Value;

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_o.FromName, _o.FromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();

        await client.ConnectAsync(_o.Host, _o.Port, SecureSocketOptions.StartTlsWhenAvailable);
        await client.AuthenticateAsync(_o.Username, _o.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
