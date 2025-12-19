using System.Net;
using System.Text.RegularExpressions;
using DnDCharacterManager.Infrastructure.Smtp.Configs;
using Infrastructure.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace DnDCharacterManager.Infrastructure.Smtp.Services;

public class EmailService(SmtpConfig config, ILogger<EmailService> logger) : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string htmlBody, string? textBody,
        CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(config.SenderName, config.Username!));
        message.To.Add(new MailboxAddress(to, to));
        message.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = htmlBody, TextBody = textBody ?? HtmlToRoughText(htmlBody) };

        message.Body = builder.ToMessageBody();

        logger.LogInformation(
            "Sending email via SMTP {Host}:{Port} From={From} To={To} Subject={Subject}",
            config.Host, config.Port, config.Username, to, subject
        );

        using var client = new SmtpClient();

        try
        {
            var options = config.Port == 587
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.SslOnConnect;

            await client.ConnectAsync(config.Host, config.Port, options, ct);

            if (!string.IsNullOrWhiteSpace(config.Username))
            {
                await client.AuthenticateAsync(config.Username, config.Password, ct);
            }

            await client.SendAsync(message, ct);

            logger.LogInformation("Email sent successfully. To={To} Subject={Subject}", to, subject);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Email send failed. To={To} Subject={Subject}", to, subject);
            throw;
        }
        finally
        {
            if (client.IsConnected)
            {
                await client.DisconnectAsync(true, ct);
            }
        }
    }

    private static string HtmlToRoughText(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return "";
        }

        var noTags = Regex.Replace(html, "<.*?>", " ");
        return WebUtility.HtmlDecode(noTags).Trim();
    }
}
