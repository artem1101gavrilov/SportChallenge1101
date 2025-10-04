using SportChallenge.DataBase;
using SportChallenge.Extensions;
using SportChallenge.Factories;
using SportChallenge.Models.Sport;
using SportChallenge.Models.User;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.Logging;

namespace SportChallenge.Services;

public partial class UserStateMachineService
{
    private Dictionary<long, UserState> _states = null!;
    private List<ActiveTrainingDto> _activeTraining = new();
    private readonly UnitOfWork _unitOfWork;
    private readonly TrainingFactory _trainingFactory;
    private readonly NotificationService _notificationController;
    private readonly ILogger<UserStateMachineService> _logger;

    public UserStateMachineService(UnitOfWork unitOfWork, TrainingFactory trainingFactory, NotificationService notificationController, ILogger<UserStateMachineService> logger)
    {
        _unitOfWork = unitOfWork;
        _trainingFactory = trainingFactory;
        _notificationController = notificationController;
        _logger = logger;
        InitializeUserStates();
    }

    private async Task GetButtonsSportType(ITelegramBotClient botClient, long chatId)
    {
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(SportType.Squats.GetRussianSportType(), ParserButtons.SerializeMenuButton(SportType.Squats)),
                InlineKeyboardButton.WithCallbackData(SportType.PushUps.GetRussianSportType(), ParserButtons.SerializeMenuButton(SportType.PushUps))
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(SportType.Crunch.GetRussianSportType(), ParserButtons.SerializeMenuButton(SportType.Crunch)),
                InlineKeyboardButton.WithCallbackData(SportType.Plank.GetRussianSportType(), ParserButtons.SerializeMenuButton(SportType.Plank))
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(SportType.BurpeeV1.GetRussianSportType(), ParserButtons.SerializeMenuButton(SportType.BurpeeV1)),
                InlineKeyboardButton.WithCallbackData(SportType.BurpeeV2.GetRussianSportType(), ParserButtons.SerializeMenuButton(SportType.BurpeeV2))
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(SportType.Scissors.GetRussianSportType(), ParserButtons.SerializeMenuButton(SportType.Scissors)),
                InlineKeyboardButton.WithCallbackData(SportType.LegSwings.GetRussianSportType(), ParserButtons.SerializeMenuButton(SportType.LegSwings))
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(SportType.PullUps.GetRussianSportType(), ParserButtons.SerializeMenuButton(SportType.PullUps))
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(SportType.Walking.GetRussianSportType(), ParserButtons.SerializeMenuButton(SportType.Walking))
            }
        });

        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Выберите упражнение:",
            replyMarkup: inlineKeyboard
        );
    }

    public bool TryProccessData(long id, string data)
    {
        if (!_states.ContainsKey(id))
            return false;

        if (ParserButtons.TryParse(data))
            return true;

        switch (_states[id])
        {
            case UserState.None:
                return false;
            case UserState.Start:
                return Enum.TryParse<SportType>(data, out _);
            case UserState.ChoosingTraining:
                return _activeTraining.Any(_ => _.Id == id)
                    && (data == Constants.Free
                    || data == Constants.Active);
            case UserState.FreeTraining:
                return int.TryParse(data, out var value) && value > 0 ||
                    data.Trim().Replace("  ", " ").Split(" ").All(__ => int.TryParse(__, out _));
            case UserState.ActiveTrainingChoosing:
            case UserState.ActiveTraining:
                return int.TryParse(data, out var valueActiveTraining) && valueActiveTraining > 0;
        }

        return ParserButtons.TryParse(data);
    }

    public async Task ProccessData(ITelegramBotClient botClient, long id, string data)
    {
        try
        {
            if (ParserButtons.TryParse(data))
            {
                switch (ParserButtons.Deserialize(data, out var result))
                {
                    case UserState.Start:
                        await ProccessStart(botClient, id, result.Last());
                        break;
                    case UserState.ChoosingTraining:
                        AddActiveTraining(id, Enum.Parse<SportType>(result.First()), 0);
                        await ProccessChoosingTraining(botClient, id, result.Last());
                        break;
                    case UserState.ActiveTrainingChoosing:
                        AddActiveTraining(id, Enum.Parse<SportType>(result.First()), 0);
                        await ProccessActiveTrainingChoosing(botClient, id, result.Last());
                        break;
                    case UserState.ActiveTraining:
                        AddActiveTraining(id, Enum.Parse<SportType>(result.First()), int.Parse(result.Skip(1).First()));
                        await ProccessActiveTraining(botClient, id, result.Last());
                        break;
                }
            }
            else
            {
                switch (_states[id])
                {
                    case UserState.None:
                        break;
                    case UserState.Start:
                        await ProccessStart(botClient, id, data);
                        return;
                    case UserState.ChoosingTraining:
                        await ProccessChoosingTraining(botClient, id, data);
                        return;
                    case UserState.FreeTraining:
                        await ProccessFreeTraining(botClient, id, data);
                        return;
                    case UserState.ActiveTrainingChoosing:
                        await ProccessActiveTrainingChoosing(botClient, id, data);
                        return;
                    case UserState.ActiveTraining:
                        await ProccessActiveTraining(botClient, id, data);
                        return;
                }

                await botClient.SendTextMessageAsync(
                    chatId: id,
                    text: "Еще не готов функцинал."
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in ProccessData for user {id}: {ex.Message}");
            await botClient.SendTextMessageAsync(
                chatId: id,
                text: "Произошла ошибка при обработке запроса. Попробуйте еще раз."
            );
        }
    }

    public async Task StartUser(ITelegramBotClient botClient, long id, string userName)
    {
        if (_states.ContainsKey(id))
        {
            _states[id] = UserState.Start;
        }
        else
        {
            _states.Add(id, UserState.Start);
            _unitOfWork.Users.Create(new(id, userName));
            await _unitOfWork.SaveChangesAsync();
        }
        await GetButtonsSportType(botClient, id);
    }

    private async Task ProccessActiveTraining(ITelegramBotClient botClient, long id, string data)
    {
        var activeTraining = _activeTraining.First(_ => _.Id == id);
        activeTraining.Count++;
        var result = await _trainingFactory.GetTraining(activeTraining.SportType)
                            .TryActiveTraining(id, activeTraining.Level, activeTraining.Count, int.Parse(data));
        if (result.result)
        {
            await botClient.SendTextMessageAsync(
                chatId: id,
                text: result.message,
                replyMarkup: new InlineKeyboardMarkup(result.inlineKeyboardButtons)
            );
            _notificationController.SetTimerForNextStepNotification(id, activeTraining);
        }
        else
        {
            _notificationController.DeleteTimer(id, activeTraining);
            _states[id] = UserState.None;
            await botClient.SendTextMessageAsync(
                chatId: id,
                text: $"Вы справились! Поздравляю!"
            );
        }
    }

    private async Task ProccessActiveTrainingChoosing(ITelegramBotClient botClient, long id, string data)
    {
        _states[id] = UserState.ActiveTraining;
        var level = int.Parse(data);
        var activeTrainingChoosing = _activeTraining.First(_ => _.Id == id);
        activeTrainingChoosing.Level = level;
        _unitOfWork.ActiveTraining.TryUpdate(id, activeTrainingChoosing.SportType, level);
        await _unitOfWork.SaveChangesAsync();
        var result = await _trainingFactory.GetTraining(activeTrainingChoosing.SportType)
                                           .TryActiveTraining(id, level, 0, 0);
        if (result.result)
        {
            await botClient.SendTextMessageAsync(
                chatId: id,
                text: result.message,
                replyMarkup: new InlineKeyboardMarkup(result.inlineKeyboardButtons)
            );
        }
    }

    private async Task ProccessFreeTraining(ITelegramBotClient botClient, long id, string data)
    {
        var freeTraining = _activeTraining.First(_ => _.Id == id);
        await _trainingFactory.GetTraining(freeTraining.SportType)
                        .FreeTraining(id, data);
        await botClient.SendTextMessageAsync(
            chatId: id,
            text: "Результат сохранен. Молодец, продолжай в том же духе!"
        );
        _states[id] = UserState.None;
    }

    private async Task ProccessChoosingTraining(ITelegramBotClient botClient, long id, string data)
    {
        var trainingChoosing = _activeTraining.First(_ => _.Id == id);
        if (data == Constants.Free && _trainingFactory.GetTraining(trainingChoosing.SportType).HaveFreeTraining())
        {
            var message = trainingChoosing.SportType switch
            {
                SportType.Plank => "Напишите сколько секунд смогли продержаться:",
                SportType.Walking => "Напишите сколько шагов прошли:",
                _ => "Напишите сколько раз смогли выполнить:"
            };
            _states[id] = UserState.FreeTraining;
            await botClient.SendTextMessageAsync(
                chatId: id,
                text: message
            );
        }
        else if (data == Constants.Active && _trainingFactory.GetTraining(trainingChoosing.SportType)
                                                     .HaveActiveTraining())
        {
            _states[id] = UserState.ActiveTrainingChoosing;
            using var photo = _trainingFactory.GetTraining(trainingChoosing.SportType)
                                              .GetActivePlanPhoto();

            var photoSent = await SendPhotoWithRetryAsync(botClient, id, new InputFileStream(photo));
            if (!photoSent)
            {
                _logger.LogWarning($"Failed to send photo for {trainingChoosing.SportType} to user {id}, continuing with text message");
            }

            var lastActiveTraining = _unitOfWork.ActiveTraining.GetAll().FirstOrDefault(_ => _.UserId == id &&
                                        _.SportType == trainingChoosing.SportType);
            var inlineKeyboard = GetActivePlanButtons(lastActiveTraining, trainingChoosing.SportType);

            await botClient.SendTextMessageAsync(
                chatId: id,
                text: "Выберите неделю (если хотите другую, то просто напишите):",
                replyMarkup: inlineKeyboard
            );
        }
        else
        {
            await botClient.SendTextMessageAsync(
                    chatId: id,
                    text: "Что-то пошло не так."
                );
        }
    }

    private async Task ProccessStart(ITelegramBotClient botClient, long id, string data)
    {
        try
        {
            if (Enum.TryParse<SportType>(data, out var result))
            {
                _states[id] = UserState.ChoosingTraining;
                AddActiveTraining(id, result, 0);

                if (_trainingFactory.GetTraining(result).HaveGifTraining())
                {
                    using var gif = _trainingFactory.GetTraining(result)
                                                    .GetGif();

                    var videoSent = await SendVideoWithRetryAsync(botClient, id, new InputFileStream(gif));
                    if (!videoSent)
                    {
                        _logger.LogWarning($"Failed to send video for {result} to user {id}, continuing with text message");
                    }
                }

                await botClient.SendTextMessageAsync(
                    chatId: id,
                    text: "Выберите режим тренировки:",
                    replyMarkup: _trainingFactory.GetTraining(result).GetMenuButtons()
                );
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    chatId: id,
                    text: "Что-то пошло не так."
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in ProccessStart for user {id}: {ex.Message}");
            await botClient.SendTextMessageAsync(
                chatId: id,
                text: "Произошла ошибка при обработке запроса. Попробуйте еще раз."
            );
        }
    }

    private void AddActiveTraining(long id, SportType result, int step)
    {
        if (_activeTraining.Any(_ => _.Id == id))
        {
            var training = _activeTraining.First(_ => _.Id == id);
            _activeTraining.Remove(training);
            training.SportType = result;
            training.Count = step;
            _activeTraining.Add(training);
        }
        else
        {
            _activeTraining.Add(new(id, result, 0, 0));
        }
    }

    private static InlineKeyboardMarkup GetActivePlanButtons(ActiveTraining? activeTraining, SportType sportType)
    {
        if (activeTraining is null)
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Начать с первой недели", ParserButtons.SerializeActivePlanButton(sportType, 1)),
                },
            });
        }
        var level = activeTraining.Level;
        return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData($"Продолжить с {level}", ParserButtons.SerializeActivePlanButton(sportType, level)),
                    InlineKeyboardButton.WithCallbackData($"Начать с {level + 1} недели", ParserButtons.SerializeActivePlanButton(sportType, level + 1)),
                },
            });
    }

    private void InitializeUserStates()
    {
        _states = _unitOfWork.Users.GetAll().ToDictionary(user => user.Id, _ => UserState.None);
    }

    private async Task<bool> SendVideoWithRetryAsync(ITelegramBotClient botClient, long chatId, InputFileStream video, int maxRetries = 3)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await botClient.SendVideoAsync(chatId, video);
                return true;
            }
            catch (RequestException ex) when (ex.Message.Contains("timeout") || ex.Message.Contains("canceled"))
            {
                _logger.LogWarning($"Attempt {attempt}/{maxRetries} failed to send video to chat {chatId}: {ex.Message}");
                
                if (attempt == maxRetries)
                {
                    _logger.LogError($"Failed to send video to chat {chatId} after {maxRetries} attempts");
                    return false;
                }
                
                // Ждем перед следующей попыткой
                await Task.Delay(TimeSpan.FromSeconds(attempt * 2));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error sending video to chat {chatId}: {ex.Message}");
                return false;
            }
        }
        return false;
    }

    private async Task<bool> SendPhotoWithRetryAsync(ITelegramBotClient botClient, long chatId, InputFileStream photo, int maxRetries = 3)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await botClient.SendPhotoAsync(chatId, photo);
                return true;
            }
            catch (RequestException ex) when (ex.Message.Contains("timeout") || ex.Message.Contains("canceled"))
            {
                _logger.LogWarning($"Attempt {attempt}/{maxRetries} failed to send photo to chat {chatId}: {ex.Message}");
                
                if (attempt == maxRetries)
                {
                    _logger.LogError($"Failed to send photo to chat {chatId} after {maxRetries} attempts");
                    return false;
                }
                
                // Ждем перед следующей попыткой
                await Task.Delay(TimeSpan.FromSeconds(attempt * 2));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error sending photo to chat {chatId}: {ex.Message}");
                return false;
            }
        }
        return false;
    }
}