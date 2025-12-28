using System.Security.Claims;
using AuthNetExample.Features.Notifications.Models.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AuthNetExample.Features.Notifications.Services;

public class TelegramBotService
{

    private readonly ITelegramBotClient _client;
    private readonly TelegramConfig _config;
    private readonly MemoryCacheService _cache;
    private readonly ApplicationDbContext _dbContext;


    public TelegramBotService(
        ITelegramBotClient client,
        IOptions<TelegramConfig> config,
        MemoryCacheService cache,
        ApplicationDbContext dbContext
    )
    {
        _config = config.Value;
        _client = client;
        _cache = cache;
        _dbContext = dbContext;
    }

    public async Task<string> GenerateSuscriptionToken(string userId)
    {
        var token = Guid.NewGuid().ToString();
        await _cache.SetAsync(token, userId, TimeSpan.FromMinutes(20));
        return token;
    }

    private async Task SuscribeUserAsync(long chatId, string token)
    {

        var existentSuscription = await _dbContext.TelegramSuscriptions
            .AnyAsync(s => s.ChatId == chatId);

        if (existentSuscription)
        {
            await _client.SendMessage(chatId, "Ya estás suscrito a las notificaciones.");
            return;
        }

        var cachedUserId = await _cache.GetAsync<string>(token);
        if (cachedUserId is null)
        {
            await _client.SendMessage(chatId, "Token inválido o expirado.");
            return;
        }

        var suscription = new TelegramSuscription
        {
            UserId = cachedUserId,
            ChatId = chatId
        };
        _dbContext.TelegramSuscriptions.Add(suscription);
        await _cache.RemoveAsync(token);
        await _dbContext.SaveChangesAsync();
        await _client.SendMessage(chatId, "Te has dado de alta en las notificaciones.");
    }

    private async Task UnsuscribeUserAsync(long chatId)
    {
        var suscription = await _dbContext.TelegramSuscriptions
            .FirstOrDefaultAsync(s => s.ChatId == chatId);

        if (suscription != null)
        {
            _dbContext.TelegramSuscriptions.Remove(suscription);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task SendNotificationToUserAsync(string userId, string message)
    {
        var suscription = await _dbContext.TelegramSuscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (suscription != null)
        {
            await _client.SendMessage(suscription.ChatId, message);
        }
    }

    public async Task HandleUpdateAsync(Update update, CancellationToken ct)
    {
        if (update.Message is not { Text: { } messageText } message) return;

        var chatId = message.Chat.Id;
        var parts = messageText.Split(' ');
        var command = parts[0];
        var token = parts.Length > 1 ? parts[1] : null;

        switch (command)
        {
            case "/start":
                if (token != null)
                {
                    await SuscribeUserAsync(chatId, token);
                }
                else
                {
                    await _client.SendMessage(chatId, "Token requerido.");
                }
                break;

            case "/unsuscribe":
                await UnsuscribeUserAsync(chatId);
                await _client.SendMessage(chatId, "Has sido dado de baja de las notificaciones.");
                break;

            default:
                await _client.SendMessage(chatId, "Comando no reconocido. Prueba con /start.");
                break;
        }
    }
}