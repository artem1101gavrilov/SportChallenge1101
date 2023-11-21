using SportChallenge.Models.Sport;

namespace SportChallenge.Extensions;

public static class EnumExtensions
{
    public static string GetRussianSportType(this SportType sportType)
        => sportType switch
        {
            SportType.Squats => "Приседания",
            SportType.PushUps => "Отжимания",
            SportType.PullUps => "Подтягивания",
            SportType.Plank => "Планка",
            SportType.Crunch => "Скручивания (пресс)",
            SportType.BurpeeV1 => "Бёрпи (hard)",
            SportType.BurpeeV2 => "Бёрпи (easy)",
            SportType.Scissors => "Ножницы",
            SportType.LegSwings => "Махи ногами",

            _ => throw new NotImplementedException(),
        };
}