using MongoDB.Bson.Serialization.Attributes;

namespace LoLModule.Models;

public class SummonerDto
{
    [BsonId]
    public string id { get; set; }
    public string accountId { get; set; }
    public string puuid { get; set; }
    public string name { get; set; }
    public int profileIconId { get; set; }
    public long revisionDate { get; set; }
    public int summonerLevel { get; set; }
}