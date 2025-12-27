using AuthNetExample.Features.Notifications.Services;
using Microsoft.Extensions.Options;

namespace AuthNetExample.Features.Notifications.Endpoints;

public static class TelegramLinkEndpoint
{
    public static void AddGetTelegramLinkEndpoint(this WebApplication app)
    {
        app.MapGet("/notifications/telegram/link", async (
            IOptions<TelegramConfig> telegramConfig,
            TelegramBotService telegramBotService) =>
        {
            var botUsername = telegramConfig.Value.BotUsername;
            var token = await telegramBotService.GenerateSuscriptionToken();

            return Results.Ok(new
            {
                Link = $"https://t.me/{botUsername}?start={token}"
            });
        })
        .RequireAuthorization();
    }
}