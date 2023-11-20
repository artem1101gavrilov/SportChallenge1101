using FitnessTelegramBot.DataBase;
using FitnessTelegramBot.Extensions;
using FitnessTelegramBot.Models.Sport;
using FitnessTelegramBot.Services;
using Telegram.Bot.Types.ReplyMarkups;

namespace FitnessTelegramBot.Training;

public abstract class SportsExerciseBase
{
    protected int[,] _activePlan = null!;
    protected readonly UnitOfWork _unitOfWork;
    protected readonly NotificationService _notificationController;
    protected virtual SportType SportType => SportType.Squats;
    protected virtual string Path => string.Empty;
    protected virtual string PathGif => string.Empty;

    protected SportsExerciseBase(UnitOfWork unitOfWork, NotificationService notificationController)
    {
        _unitOfWork = unitOfWork;
        _notificationController = notificationController;
    }

    public virtual bool HaveFreeTraining()
        => false;

    public virtual bool HaveActiveTraining()
        => false;

    public virtual bool HaveGifTraining()
        => false;

    public virtual async Task<string> FreeTraining(long chatId, string data)
    {
        if (int.TryParse(data, out var count))
        {
            return await FreeTraining(chatId, count);
        }
        else
        {
            return await CustomTraining(chatId, data);
        }
    }

    private async Task<string> FreeTraining(long chatId, int count)
    {
        _unitOfWork.SportsExercises.TryUpdate(chatId, SportType, count);
        var delayTask = _notificationController.Notify(SportType, chatId, count);

        if (_unitOfWork.RecordsOneTime.TryUpdate(chatId, SportType, count))
        {
            var delayTask2 = _notificationController.Record(SportType, chatId, count);
        }
        await _unitOfWork.SaveChangesAsync();
        return "Результат сохранен. Молодец, продолжай в том же духе!";
    }

    public virtual async Task<(bool result, IEnumerable<IEnumerable<InlineKeyboardButton>> inlineKeyboardButtons, string message)> TryActiveTraining(long chatId, int level, int step, int count)
    {
        if (count != 0)
        {
            await FreeTraining(chatId, count);
        }
        if (step == _activePlan.GetLength(1))
        {
            return (false, Enumerable.Empty<IEnumerable<InlineKeyboardButton>>(), string.Empty);
        }
        var countTraining = _activePlan[level - 1, step];
        var inlineKeyboard = new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData($"{countTraining}", ParserButtons.SerializeCountButton(SportType, step, countTraining)),
            },
        };
        var message = $"Выполните упражнение {countTraining} раз:";
        return (true, inlineKeyboard, message);
    }

    public virtual InlineKeyboardMarkup GetMenuButtons()
        => InlineKeyboardMarkup.Empty();

    public virtual MemoryStream GetActivePlanPhoto()
    {
        using var fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read);
        var memoryStream = new MemoryStream();
        fileStream.CopyTo(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }

    public virtual MemoryStream GetGif()
    {
        using var fileStream = new FileStream(PathGif, FileMode.Open, FileAccess.Read);
        var memoryStream = new MemoryStream();
        fileStream.CopyTo(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }

    private async Task<string> CustomTraining(long chatId, string message)
    {
        var result = message.Trim().Replace("  ", " ").Split(" ");
        if (result.All(__ => int.TryParse(__, out _)))
        {
            var sportsExercise = result.Select(_ => int.Parse(_));
            var count = sportsExercise.Sum();
            _unitOfWork.SportsExercises.TryUpdate(chatId, SportType, count);
            var delayTask = _notificationController.Notify(SportType, chatId, count, sportsExercise.Count());

            var max = sportsExercise.Max();
            if (_unitOfWork.RecordsOneTime.TryUpdate(chatId, SportType, max))
            {
                var delayTask2 = _notificationController.Record(SportType, chatId, max);
            }
            await _unitOfWork.SaveChangesAsync();
            return "Результат сохранен. Молодец, продолжай в том же духе!";
        }

        return "Что-то пошло не так.";
    }
}