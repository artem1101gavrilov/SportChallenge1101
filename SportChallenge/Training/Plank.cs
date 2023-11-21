using SportChallenge.DataBase;
using SportChallenge.Extensions;
using SportChallenge.Models.Sport;
using SportChallenge.Services;
using Telegram.Bot.Types.ReplyMarkups;

namespace SportChallenge.Training;

public class Plank : SportsExerciseBase
{
    protected override SportType SportType => SportType.Plank;

    public Plank(UnitOfWork unitOfWork, NotificationService notificationController) : base(unitOfWork, notificationController)
    {
    }

    public override bool HaveFreeTraining()
        => true;

    public override async Task<string> FreeTraining(long chatId, string data)
    {
        if (int.TryParse(data, out var count))
        {
            if (_unitOfWork.RecordsOneTime.TryUpdate(chatId, SportType, count))
            {
                var delayTask = _notificationController.Record(SportType, chatId, count);
                await _unitOfWork.SaveChangesAsync();
            }
            var delayTask2 = _notificationController.Notify(SportType, chatId, count);
            await Task.CompletedTask;
            return "Результат сохранен. Молодец, продолжай в том же духе!";
        }
        else
        {
            return "Что-то пошло не так.";
        }
    }

    public override InlineKeyboardMarkup GetMenuButtons()
        => new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("На рекорд", ParserButtons.SerializeSportMenuButton(SportType, Constants.Free)),
            }
        });
}