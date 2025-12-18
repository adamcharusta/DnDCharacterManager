using System;
using System.Threading.Tasks;
using DnDCharacterManager.Contracts.Notifications;

namespace Services.Handlers;

public static class SendEmailHandler
{
    public static Task Handle(SendEmail cmd)
    {
        Console.WriteLine(
            $"EMAIL → {cmd.To} | {cmd.Subject} | {cmd.Body}"
        );

        return Task.CompletedTask;
    }
}
