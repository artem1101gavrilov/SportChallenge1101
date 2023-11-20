using Telegram.Bot.Types.ReplyMarkups;
using FitnessTelegramBot.Extensions;
using FitnessTelegramBot.Services;
using FitnessTelegramBot.DataBase;
using FitnessTelegramBot.Models.Sport;

namespace FitnessTelegramBot.Training;

public class Squats : SportsExerciseBase
{
    protected override SportType SportType => SportType.Squats;
    protected override string Path => "Files/Squats.jpg";
    protected override string PathGif => "Files/Squats_gif.mp4";

    public Squats(UnitOfWork unitOfWork, NotificationService notificationController) : base(unitOfWork, notificationController)
    {
        _activePlan = new int[,]
        {
            { 8, 10, 8, 8, 6 },
            { 10, 12, 10, 10, 8 },
            { 10, 15, 12, 10, 10 },
            { 15, 15, 15, 15, 12 },
            { 15, 20, 18, 16, 12 },
            { 15, 20, 20, 20, 15 },
            { 20, 25, 20, 20, 20 },
            { 20, 25, 25, 25, 20 },
            { 25, 30, 30, 25, 20 },
            { 25, 30, 30, 30, 25 },
            { 30, 30, 30, 30, 30 },
            { 30, 35, 35, 30, 30 },
            { 30, 40, 35, 35, 30 },
            { 35, 40, 35, 35, 35 },
            { 40, 40, 40, 40, 40 },
        };
    }

    public override bool HaveActiveTraining()
        => true;

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
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Активный", ParserButtons.SerializeSportMenuButton(SportType, Constants.Active)),
            }
        });
}