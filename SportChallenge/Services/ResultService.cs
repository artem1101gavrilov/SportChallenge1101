using SportChallenge.DataBase;
using SportChallenge.Extensions;
using Telegram.Bot;

namespace SportChallenge.Services;

public class ResultService
{
    private readonly UnitOfWork _unitOfWork;

    public ResultService(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public bool TryParse(string message)
    {
        if (message == "/recordtoday" ||
            message == "/record" ||
            message == "/recordonetime" ||
            message == "/myresult")
        {
            return true;
        }

        return false;
    }

    public async Task Parse(string message, ITelegramBotClient botClient, long chatId)
    {
        if (message == "/recordtoday")
        {
            await GetRecordsToday(botClient, chatId);
        }
        else if (message == "/record")
        {
            await GetRecordsAll(botClient, chatId);
        }
        else if (message == "/recordonetime")
        {
            await GetRecordsOneTimeAll(botClient, chatId);
        }
        else if (message == "/myresult")
        {
            await GetMyResult(botClient, chatId);
        }
        else
        {
            throw new ArgumentNullException(nameof(message));
        }
    }

    public async Task GetRecordsToday(ITelegramBotClient botClient, long chatId)
    {
        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: _unitOfWork.SportsExercises.GetAll().GetRecordsToday(_unitOfWork.Users.GetAll()) ?? "Рекордов нет"
        );
    }

    public async Task GetRecordsAll(ITelegramBotClient botClient, long chatId)
    {
        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: _unitOfWork.SportsExercises.GetAll().GetRecordsAll(_unitOfWork.Users.GetAll()) ?? "Рекордов нет"
        );
    }

    public async Task GetMyResult(ITelegramBotClient botClient, long chatId)
    {
        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: _unitOfWork.RecordsOneTime.GetAll().GetMyResult(_unitOfWork.SportsExercises.GetAll(), chatId) ?? "Рекордов нет"
        );
    }

    public async Task GetRecordsOneTimeAll(ITelegramBotClient botClient, long chatId)
    {
        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: _unitOfWork.RecordsOneTime.GetAll().GetRecordsOneTimeAll(_unitOfWork.Users.GetAll()) ?? "Рекордов нет"
        );
    }
}