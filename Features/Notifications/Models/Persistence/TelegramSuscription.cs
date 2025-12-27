namespace AuthNetExample.Features.Notifications.Models.Persistence;

public class TelegramSuscription
{
    public required string UserId { get; set; }
    public required long ChatId { get; set; }
}