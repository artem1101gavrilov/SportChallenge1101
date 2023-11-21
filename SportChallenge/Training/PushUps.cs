using Telegram.Bot.Types.ReplyMarkups;
using SportChallenge.Extensions;
using SportChallenge.Services;
using SportChallenge.DataBase;
using SportChallenge.Models.Sport;

namespace SportChallenge.Training;

public class PushUps : SportsExerciseBase
{
    protected override SportType SportType => SportType.PushUps;
    protected override string Path => "Files/PushUps.jpg";
    protected override string PathGif => "Files/PushUps_gif.mp4";


    public PushUps(UnitOfWork unitOfWork, NotificationService notificationController) : base(unitOfWork, notificationController)
    {
        _activePlan = new int[,]
        {
            { 6, 5, 5, 4, 3 },
            { 7, 6, 5, 4, 4 },
            { 8, 6, 5, 5, 4 },
            { 8, 7, 5, 5, 5 },
            { 9, 7, 6, 5, 5 },
            { 10, 7, 6, 6, 5 },
            { 10, 8, 6, 6, 6 },
            { 11, 8, 7, 6, 6 },
            { 12, 8, 7, 7, 6 },
            { 12, 9, 7, 7, 7 },
            { 13, 9, 8, 7, 7 },
            { 14, 9, 8, 8, 7 },
            { 14, 10, 8, 8, 8 },
            { 15, 10, 9, 8, 8 },
            { 16, 10, 9, 9, 8 },
            { 16, 11, 9, 9, 9 },
            { 17, 11, 10, 9, 9 },
            { 18, 11, 10, 10, 9 },
            { 18, 12, 10, 10, 10 },
            { 19, 12, 11, 10, 10 },
            { 20, 12, 11, 11, 10 },
            { 20, 13, 11, 11, 11 },
            { 21, 13, 12, 11, 11 },
            { 22, 13, 12, 12, 11 },
            { 22, 14, 12, 12, 12 },
            { 23, 14, 13, 12, 12 },
            { 24, 14, 13, 13, 12 },
            { 24, 15, 13, 13, 13 },
            { 25, 15, 14, 13, 13 },
            { 26, 15, 14, 14, 13 },
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