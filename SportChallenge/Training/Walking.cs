using Telegram.Bot.Types.ReplyMarkups;
using SportChallenge.Extensions;
using SportChallenge.Services;
using SportChallenge.DataBase;
using SportChallenge.Models.Sport;

namespace SportChallenge.Training;

public class Walking : SportsExerciseBase
{
    protected override SportType SportType => SportType.Walking;

    public Walking(UnitOfWork unitOfWork, NotificationService notificationController) : base(unitOfWork, notificationController)
    {
    }

    public override bool HaveFreeTraining()
        => true;

    public override InlineKeyboardMarkup GetMenuButtons()
        => new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Свободный", ParserButtons.SerializeSportMenuButton(SportType, Constants.Free)),
            }
        });
}
