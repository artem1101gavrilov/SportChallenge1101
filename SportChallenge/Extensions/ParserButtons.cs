using FitnessTelegramBot.Models.Sport;
using FitnessTelegramBot.Models.User;

namespace FitnessTelegramBot.Extensions;

public static class ParserButtons
{
    private const string Prefix = "SC1101;";
    private const string SportMenu = nameof(SportMenu);
    private const string Menu = nameof(Menu);
    private const string ActivePlan = nameof(ActivePlan);
    private const string Count = nameof(Count);

    public static bool TryParse(string message)
    {
        if (message.StartsWith(Prefix))
        {
            return true;
        }
        return false;
    }

    public static UserState Deserialize(string message, out string[] data)
    {
        var result = message.Split(';');
        data = result.Skip(2).ToArray();

        if (result[1] == SportMenu)
        {
            return UserState.ChoosingTraining;
        }
        else if (result[1] == ActivePlan)
        {
            return UserState.ActiveTrainingChoosing;
        }
        else if (result[1] == Count)
        {
            return UserState.ActiveTraining;
        }
        else if (result[1] == Menu)
        {
            return UserState.Start;
        }
        else
        {
            throw new Exception($"{nameof(Deserialize)} - {message}");
        }
    }

    public static string SerializeSportMenuButton(SportType sportType, string type)
        => Serialize($"{SportMenu};{sportType};{type}");

    public static string SerializeMenuButton(SportType sportType)
        => Serialize($"{Menu};{sportType}");

    public static string SerializeActivePlanButton(SportType sportType, int week)
        => Serialize($"{ActivePlan};{sportType};{week}");

    public static string SerializeCountButton(SportType sportType, int step, int count)
        => Serialize($"{Count};{sportType};{step};{count}");

    private static string Serialize(string message)
        => Prefix + message;
}