using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SportChallenge.Controllers;
using SportChallenge.DataBase;
using SportChallenge.Extensions;
using SportChallenge.Factories;
using SportChallenge.Logger;
using SportChallenge.Services;
using Telegram.Bot;

// todo list:
/*
3) Добавить в рекорды еще места 1) 2) 3) И если будет более 5 человек в категории, то выводить лишь пятерку и ниже свое место
5) Вес и его динамику каждый месяц.
7) Сколько сжёг калорий.
8) команда для отключения получения фотографий/гифок.
10) Кнопка по настройке Го!
11) Добавить гантели
12) Уведомлять человека, что он не занимался Н дней.
13) Уведомлять, когда не пробовал какой-либо спорт.
14) Убрать логирование ДБ в консоли. 
16) Удаление последнего результата.
17) Free тренировка во время выключенного бота
18) Для планки пришел, что результат сохранен, хотя рекорд не побит и он никуда не сохранен, надо изменить текст.
19) Уведомление, когда за все время перегнал кто-то.
 */

var serviceProvider = new ServiceCollection()
        .AddDbContext<ApplicationContext>(options =>
        {
            options.UseSqlite("Data Source=helloapp1.db");
            options.LogTo(_ => { });
        })
        .AddLogging(_ => _.AddProvider(new FileLoggerProvider("log/log2.txt")))
        .AddSingleton<TelegramController>()
        .AddSingleton<UserStateMachineService>()
        .AddSingleton<NotificationService>()
        .AddSingleton<TrainingFactory>()
        .AddSingleton<ResultService>()
        .AddSingleton<UnitOfWork>()
        .AddSingleton<ITelegramBotClient>(provider =>
        {
            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(5) // Увеличиваем таймаут до 5 минут
            };
            return new TelegramBotClient(TokenParser.GetToken(), httpClient);
        })
        .BuildServiceProvider();


var telegramController = serviceProvider.GetService<TelegramController>();
try
{
    await telegramController!.Start();
}
catch (Exception ex)
{
    var logger = serviceProvider.GetService<ILogger<TelegramController>>();
    logger?.LogError($"Fatal error in bot: {ex}");
    Console.WriteLine($"Fatal error: {ex.Message}");
    Environment.Exit(1);
}

//using var db = serviceProvider.GetService<ApplicationContext>();

//var result = db.SportsExercises.OrderByDescending(_ => _.Date).FirstOrDefault(_ => _.UserId == 5179978672 && _.SportType == SportChallenge.Models.Sport.SportType.Squats);

////result.Count = 51;
//db.SportsExercises.Remove(result);
//db.SaveChanges();