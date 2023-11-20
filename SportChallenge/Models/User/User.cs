namespace FitnessTelegramBot.Models.User;

public record class User(long Id, string UserName, bool IsVisible = true);