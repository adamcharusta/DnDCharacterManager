using Ardalis.GuardClauses;
using DnDCharacterManager.Infrastructure.Smtp.Configs;
using DnDCharacterManager.Infrastructure.Smtp.Services;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DnDCharacterManager.Infrastructure.Smtp;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureSmtp(this IServiceCollection services,
        ConfigurationManager configurationManager)
    {
        var smtpConfig = new SmtpConfig();
        configurationManager.GetSection("SmtpConfig").Bind(smtpConfig);

        Guard.Against.NullOrEmpty(smtpConfig.Host);
        Guard.Against.NegativeOrZero(smtpConfig.Port);
        Guard.Against.NullOrEmpty(smtpConfig.Username);
        Guard.Against.NullOrEmpty(smtpConfig.Password);
        Guard.Against.NullOrEmpty(smtpConfig.SenderName);

        services.AddSingleton(smtpConfig);
        services.AddSingleton<IEmailService, EmailService>();

        return services;
    }
}
