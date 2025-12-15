namespace DnDCharacterManager.Contracts.Notifications;

public record SendEmail(
    string To,
    string Subject,
    string Body
);
