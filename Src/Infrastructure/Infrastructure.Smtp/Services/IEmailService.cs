namespace Infrastructure.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody, string? textBody,
        CancellationToken ct = default);
}
