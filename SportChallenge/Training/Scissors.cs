using Telegram.Bot.Types.ReplyMarkups;
using FitnessTelegramBot.Extensions;
using FitnessTelegramBot.Services;
using FitnessTelegramBot.DataBase;
using FitnessTelegramBot.Models.Sport;

namespace FitnessTelegramBot.Training;

public class Scissors : SportsExerciseBase
{
    protected override SportType SportType => SportType.Scissors;
    protected override string PathGif => "Files/Scissors_gif.mp4";

    public Scissors(UnitOfWork unitOfWork, NotificationService notificationController) : base(unitOfWork, notificationController)
    {
    }

    public override bool HaveFreeTraining()
        => true;

    public override bool HaveGifTraining()
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
