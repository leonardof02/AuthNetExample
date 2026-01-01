using Jobs.Backend.Features.Notifications.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Jobs.Backend.Features.Notifications.Workers;

public class TelegramNotificationBackgroundService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TelegramNotificationBackgroundService> _logger;

    public TelegramNotificationBackgroundService(
        ITelegramBotClient botClient, 
        IServiceProvider serviceProvider,
        ILogger<TelegramNotificationBackgroundService> logger)
    {
        _botClient = botClient;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Iniciando Bot de Telegram...");

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: stoppingToken
        );
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<TelegramBotService>();
            await handler.HandleUpdateAsync(update, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando update de Telegram");
            await _botClient.SendMessage(update.Message!.Chat.Id, "Ocurrió un error al procesar tu solicitud.");
        }
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken ct)
    {
        _logger.LogError(exception, "Error en el polling de Telegram");
        return Task.CompletedTask;
    }
}