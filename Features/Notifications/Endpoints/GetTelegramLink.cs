using System.Security.Claims;
using AuthNetExample.Features.Notifications.Services;
using Microsoft.Extensions.Options;

namespace AuthNetExample.Features.Notifications.Endpoints;

public static class TelegramLinkEndpoint
{
    public static void AddGetTelegramLinkEndpoint(this WebApplication app)
    {
        app.MapGet("/notifications/telegram/suscribe", async (
            IOptions<TelegramConfig> telegramConfig,
            HttpContext httpContext,
            TelegramBotService telegramBotService) =>
        {
            var botUsername = telegramConfig.Value.BotUsername;
            var userId = httpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new Exception("User is not authenticated.");
            var token = await telegramBotService.GenerateSuscriptionToken(userId);

            return Results.Ok(new
            {
                Link = $"https://t.me/{botUsername}?start={token}"
            });
        })
        .RequireAuthorization();
    }
}