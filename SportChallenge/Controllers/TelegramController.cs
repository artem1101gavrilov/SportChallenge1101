using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Microsoft.Extensions.Logging;
using FitnessTelegramBot.Services;

namespace FitnessTelegramBot.Controllers;

public class TelegramController
{
    private readonly UserStateMachineService _userStateMachineController;
    private readonly ResultService _resultController;
    private readonly ILogger<TelegramController> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly NotificationService _notificationController;
    private CancellationTokenSource _cancellationToken = null!;

    public TelegramController(UserStateMachineService userStateMachineController, ResultService resultController, ITelegramBotClient botClient, NotificationService notificationController, ILogger<TelegramController> logger)
    {
        _botClient = botClient;
        _notificationController = notificationController;
        _userStateMachineController = userStateMachineController;
        _resultController = resultController;
        _logger = logger;
    }

    public async Task Start()
    {
        using (_cancellationToken = new())
        {
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: _cancellationToken.Token
            );

            var me = await _botClient.GetMeAsync();

            await ChengeBotName("🔋 Sport Challenge 1101");
            //var delayTask = _notificationController.WhatsNew();

            _logger.LogInformation($"Start listening for @{me.Username}");
            Console.ReadLine();

            await ChengeBotName("\U0001faab Sport Challenge 1101");

            _cancellationToken.Cancel();
        }
    }

    private async Task ChengeBotName(string name)
    {
        try
        {
            await _botClient.SetMyNameAsync(name);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex.ToString());
        }
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update is null
            || (update.Message is null
            && update.CallbackQuery is null))
            return;

        var messageText = update.Message?.Text ?? update.CallbackQuery?.Data;
        if (messageText == null) return;
        var chatId = update.Message?.Chat.Id ?? update.CallbackQuery!.From.Id;
        var userName = update.Message is not null
                        ? $"{update.Message?.Chat.FirstName} {update.Message?.Chat.LastName}"
                        : $"{update.CallbackQuery!.From.FirstName} {update.CallbackQuery!.From.LastName}";

        _logger.LogInformation($"Received a '{messageText}' message in chat {chatId} - {userName}.");

        if (_resultController.TryParse(messageText))
        {
            await _resultController.Parse(messageText, botClient, chatId);
        }
        else if (messageText == "/start")
        {
            await _userStateMachineController.StartUser(botClient, chatId, userName);
        }
        else if (_userStateMachineController.TryProccessData(chatId, messageText!))
        {
            try
            {
                await _userStateMachineController.ProccessData(botClient, chatId, messageText!);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.ToString());
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Что-то пошло не так."
                );
            }
        }
        else
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Не смог обработать кнопку или текст, обратитись к администратору."
            );
        }
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation(ErrorMessage);
        return Task.CompletedTask;
    }
}