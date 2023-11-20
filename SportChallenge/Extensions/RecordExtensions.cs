using FitnessTelegramBot.Models.Sport;
using FitnessTelegramBot.Models.User;
using System.Text;

namespace FitnessTelegramBot.Extensions;

public static class RecordExtensions
{
    public static string? GetRecordsToday(this IEnumerable<SportsExercise> sportsExercises, IEnumerable<User> users)
    {
        var today = DateTime.Today;

        var recordsTodayGroupedBySportType = sportsExercises.ToArray()
            .Where(exercise => exercise.Date.Date == today)
            .GroupBy(exercise => exercise.SportType)
            .Select(group => new
            {
                SportType = group.Key,
                Users = group.GroupBy(exercise => exercise.UserId)
                    .Select(userGroup => new
                    {
                        UserId = userGroup.Key,
                        Count = userGroup.Sum(_ => _.Count)
                    })
            });

        StringBuilder stringBuilder = new();
        foreach (var result in recordsTodayGroupedBySportType)
        {
            stringBuilder.AppendLine(result.SportType.GetRussianSportType());

            foreach (var userCount in result.Users.OrderByDescending(_ => _.Count))
            {
                var user = users.First(user => user.Id == userCount.UserId);
                if (user.IsVisible)
                {
                    stringBuilder.AppendLine($"{user.UserName} : {userCount.Count}");
                }
            }

            stringBuilder.AppendLine();
        }
        var records = stringBuilder.ToString();
        return string.IsNullOrEmpty(records) ? null : records;
    }

    public static string? GetRecordsAll(this IEnumerable<SportsExercise> sportsExercises, IEnumerable<User> users)
    {
        var recordsGroupedBySportType = sportsExercises.ToArray()
            .GroupBy(exercise => exercise.SportType)
            .Select(group => new
            {
                SportType = group.Key,
                Users = group.GroupBy(exercise => exercise.UserId)
                    .Select(userGroup => new
                    {
                        UserId = userGroup.Key,
                        Count = userGroup.Sum(_ => _.Count)
                    })
            });

        StringBuilder stringBuilder = new();

        foreach (var result in recordsGroupedBySportType)
        {
            stringBuilder.AppendLine(result.SportType.GetRussianSportType());

            foreach (var userCount in result.Users.OrderByDescending(_ => _.Count))
            {
                var user = users.First(user => user.Id == userCount.UserId);
                if (user.IsVisible)
                {
                    stringBuilder.AppendLine($"{user.UserName} : {userCount.Count}");
                }
            }

            stringBuilder.AppendLine();
        }
        var records = stringBuilder.ToString();
        return string.IsNullOrEmpty(records) ? null : records;
    }

    public static string? GetRecordsOneTimeAll(this IEnumerable<RecordsOneTime> recordsOneTimes, IEnumerable<User> users)
    {
        var recordsGroupedBySportType = recordsOneTimes.ToArray()
            .GroupBy(exercise => exercise.SportType)
            .Select(group => new
            {
                SportType = group.Key,
                Users = group.GroupBy(exercise => exercise.UserId)
                    .Select(userGroup => new
                    {
                        UserId = userGroup.Key,
                        Date = group.FirstOrDefault(_ => _.UserId == userGroup.Key)?.Date ?? DateTime.Now,
                        Count = userGroup.Sum(_ => _.Count)
                    })
            });

        StringBuilder stringBuilder = new();

        foreach (var result in recordsGroupedBySportType)
        {
            stringBuilder.AppendLine(result.SportType.GetRussianSportType());

            foreach (var userCount in result.Users.OrderByDescending(_ => _.Count))
            {
                var user = users.First(user => user.Id == userCount.UserId);
                if (user.IsVisible)
                {
                    stringBuilder.AppendLine($"{user.UserName} [{userCount.Date.ToShortDateString()}] : {userCount.Count}");
                }
            }

            stringBuilder.AppendLine();
        }
        var records = stringBuilder.ToString();
        return string.IsNullOrEmpty(records) ? null : records;
    }

    public static string? GetMyResult(this IEnumerable<RecordsOneTime> recordsOneTimes, IEnumerable<SportsExercise> sportsExercises, long id)
    {
        var myRecordsOneTimes = recordsOneTimes.Where(_ => _.UserId == id)
                                               .ToArray();

        var myRecordsSportsExercises = sportsExercises.Where(_ => _.UserId == id)
                                                      .GroupBy(_ => _.SportType)
                                                      .Select(_ => new
                                                      {
                                                          SportType = _.Key,
                                                          Count = _.Sum(x => x.Count),
                                                      })
                                                      .ToArray();

        StringBuilder stringBuilder = new("Рекорды за один подход:\n");
        foreach (var result in myRecordsOneTimes)
        {
            stringBuilder.AppendLine($"{result.SportType.GetRussianSportType()} - {result.Count}");
        }

        stringBuilder.AppendLine();
        stringBuilder.AppendLine("Результаты:");
        foreach (var result in myRecordsSportsExercises)
        {
            stringBuilder.AppendLine($"{result.SportType.GetRussianSportType()} - {result.Count}");
        }

        var records = stringBuilder.ToString();
        return string.IsNullOrEmpty(records) ? null : records;
    }
}