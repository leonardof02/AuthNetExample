public record TelegramConfig
{
    public string BotUsername { get; init; } = string.Empty;
    public string BotToken { get; init; } = string.Empty;
}