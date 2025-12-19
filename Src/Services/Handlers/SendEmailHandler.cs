using DnDCharacterManager.Contracts.Notifications;
using DnDCharacterManager.Infrastructure.Smtp.Templates;
using Infrastructure.Services;

namespace Services.Handlers;

public class SendEmailHandler
{
    public static async Task Handle(SendEmail cmd, IEmailService emailService,
        ILogger<SendEmailHandler> logger, CancellationToken ct)
    {
        var model = new EmailTemplate { Subject = cmd.Subject, Body = cmd.Body };

        var htmlBody = await model.RenderAsync(ct);

        await emailService.SendEmailAsync(cmd.To, cmd.Subject, htmlBody, null, ct);
    }
}
