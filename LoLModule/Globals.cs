namespace LoLModule;

public static class Globals
{
    public const string BaseUrl = "https://na1.api.riotgames.com/";
    public const string BaseAmericaUrl = "https://americas.api.riotgames.com/";
    public static readonly string ApiKey = Environment.GetEnvironmentVariable("API_KEY_LOL") ?? throw new Exception("LOL API KEY NOT SET");
    public static readonly string ChampionInfoUrl = "http://ddragon.leagueoflegends.com/cdn/6.24.1/data/en_US/champion.json";
    public const string ItemInfoUrl = "http://ddragon.leagueoflegends.com/cdn/13.3.1/data/en_US/item.json";
    public const string QueueTypeUrl = "https://static.developer.riotgames.com/docs/lol/queues.json";
}