using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Infrastructure;

public class TelegramBotBackgroundService : BackgroundService
{
    public TelegramBotBackgroundService(
        ILogger<TelegramBotBackgroundService> logger,
        IServiceProvider serviceProvider) 
    {
        _logger = logger;
        _botClient = serviceProvider.GetService<ITelegramBotClient>()!;
    }

    private readonly ILogger<TelegramBotBackgroundService> _logger;

    private readonly ITelegramBotClient _botClient;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }
        };

        _botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            stoppingToken);

        _logger.LogInformation("Telegram bot started");

        try
        {
            await Task.Delay(-1, stoppingToken);
        }
        catch (TaskCanceledException taskCanceledException)
        {
            Console.WriteLine(taskCanceledException);
        }
    }

    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message)
            return;
        if (message.Text is not { } messageText)
            return;

        var chatId = message.Chat.Id;

        Console.WriteLine($"Received a '{messageText}' message in chat {chatId} from @{message.From!.Username};");

        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: $"Your id: {chatId}",
            cancellationToken: cancellationToken);
        
        await botClient.SetChatMenuButtonAsync(
            chatId: chatId,
            menuButton: new MenuButtonWebApp() { Text = "Play", WebApp = new WebAppInfo { Url = "https://www.youtube.com/" } },
            cancellationToken: cancellationToken);
    }

    Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error: \n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
            _ => exception.ToString()
        };
        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}
