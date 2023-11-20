using Telegram.Bot.Types.ReplyMarkups;
using FitnessTelegramBot.Extensions;
using FitnessTelegramBot.Services;
using FitnessTelegramBot.DataBase;
using FitnessTelegramBot.Models.Sport;

namespace FitnessTelegramBot.Training;

public class BurpeeV1 : SportsExerciseBase
{
    protected override SportType SportType => SportType.BurpeeV1;
    protected override string PathGif => "Files/BurpeeV1_gif.mp4";

    public BurpeeV1(UnitOfWork unitOfWork, NotificationService notificationController) : base(unitOfWork, notificationController)
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
