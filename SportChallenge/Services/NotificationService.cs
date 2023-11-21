using SportChallenge.DataBase;
using SportChallenge.Extensions;
using SportChallenge.Models.Sport;
using Telegram.Bot;

namespace SportChallenge.Services;

public class NotificationService
{
    private readonly ITelegramBotClient _botClient;
    private readonly UnitOfWork _unitOfWork;
    private List<SportType> _sportTypes = ((SportType[])Enum.GetValues(typeof(SportType))).ToList();
    private List<(long Id, SportType SportType, int Level, int Step)> _timersGo = new();

    public NotificationService(ITelegramBotClient botClient, UnitOfWork unitOfWork)
    {
        _botClient = botClient;
        _unitOfWork = unitOfWork;
    }

    public async Task WhatsNew()
    {
        var users = _unitOfWork.Users.GetAll();
        var message = "Что нового?\n";

        foreach (var user in users)
        {
            var delayTask = _botClient.SendTextMessageAsync(
            chatId: user.Id,
                text: message
            );
        }
        await Task.Yield();
    }

    public async Task Notify(SportType sportType, long chatId, int count)
    {
        if (_sportTypes.Contains(sportType))
        {
            _sportTypes.Remove(sportType);
            var sportsmen = _unitOfWork.Users.Get(u => u.Id == chatId);
            var today = DateTime.Today;
            var activeUsers = _unitOfWork.SportsExercises.GetCollection(_ => _.Date.Date == today)
                                                         .Select(_ => _.UserId)
                                                         .Append(chatId)
                                                         .Distinct();
            var users = _unitOfWork.Users.GetCollection(_ => !activeUsers.Contains(_.Id));

            var message = sportType == SportType.Plank
                            ? $"{sportsmen.UserName} сделал/а упражнение {sportType.GetRussianSportType()} и продержался(-ась) {count} секунд. Может пора тоже пострадать?"
                            : $"{sportsmen.UserName} сделал/а упражнение {sportType.GetRussianSportType()} {count} раз. Может пора тоже позаниматься, ленивая жопа?";

            foreach (var user in users)
            {
                var delayTask = _botClient.SendTextMessageAsync(
                    chatId: user.Id,
                    text: message
                );
            }
            await Task.Yield();
        }
        else
        {
            await Task.Yield();
        }
    }

    public async Task Notify(SportType sportType, long chatId, int count, int step)
    {
        if (_sportTypes.Contains(sportType))
        {
            _sportTypes.Remove(sportType);
            var sportsmen = _unitOfWork.Users.Get(u => u.Id == chatId);
            var today = DateTime.Today;
            var activeUsers = _unitOfWork.SportsExercises.GetCollection(_ => _.Date.Date == today)
                                                         .Select(_ => _.UserId)
                                                         .Distinct();
            var users = _unitOfWork.Users.GetCollection(_ => !activeUsers.Contains(_.Id));

            var message = $"{sportsmen.UserName} сделал/а упражнение {sportType.GetRussianSportType()} {count} раз за {step} подходов. Может пора тоже позаниматься, ленивая жопа?";

            foreach (var user in users)
            {
                var delayTask = _botClient.SendTextMessageAsync(
                    chatId: user.Id,
                    text: message
                );
            }
            await Task.Yield();
        }
        else
        {
            await Task.Yield();
        }
    }

    public async Task StartNextStepActiveSportExercise(long chatId) 
        => await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Го! Прошло 2 минуты с прошлого подхода!"
            );

    public void DeleteTimer(long id, ActiveTrainingDto activeTraining)
    {
        if (_timersGo.Contains((id, activeTraining.SportType, activeTraining.Level, activeTraining.Count - 1)))
        {
            _timersGo.Remove((id, activeTraining.SportType, activeTraining.Level, activeTraining.Count - 1));
        }
    }

    public void SetTimerForNextStepNotification(long id, ActiveTrainingDto activeTraining)
    {
        DeleteTimer(id, activeTraining);

        var timerData = (id, activeTraining.SportType, activeTraining.Level, activeTraining.Count);
        _timersGo.Add(timerData);
        var timer = new System.Timers.Timer(2 * 60 * 1000);
        
        timer.Elapsed += async (sender, _) =>
        {
            if (sender is System.Timers.Timer timerSender)
            {
                timerSender.Stop();
            }
            if (_timersGo.Contains(timerData))
            {
                _timersGo.Remove(timerData);
                await StartNextStepActiveSportExercise(id);
            }
        };
        timer.Start();
    }

    public async Task Record(SportType sportType, long chatId, int count)
    {
        var sportsmen = _unitOfWork.Users.Get(u => u.Id == chatId);
        var users = _unitOfWork.Users.GetCollection(_ => _.Id != chatId);

        var message = sportType == SportType.Plank
                        ? $"{sportsmen.UserName} установил/а рекорд в упражнении {sportType.GetRussianSportType()} и продержался {count} секунд."
                        : $"{sportsmen.UserName} установил/а рекорд в упражнении {sportType.GetRussianSportType()}, сделав {count} раз.";

        foreach (var user in users)
        {
            var delayTask = _botClient.SendTextMessageAsync(
                chatId: user.Id,
                text: message
            );
        }
        await Task.Yield();
    }
}