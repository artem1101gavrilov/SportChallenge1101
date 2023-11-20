using System.Text.Json;

namespace SportChallenge.Extensions;

public static class TokenParser
{
    private record struct Token(string Key);

    public static string GetToken()
    {
        var json = File.ReadAllText("Token.json");
        return JsonSerializer.Deserialize<Token>(json)!.Key;
    }
}