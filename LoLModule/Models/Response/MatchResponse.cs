using LoLModule.Models.Dto;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LoLModule.Models.Response;

public class MatchResponse : MatchDto
{
    [BsonId]
    public string Id { get; set; }
    public ParticipantResponse focusedPlayer { get; set; }
    public TeamResponse teammates { get; set; }
    public TeamResponse opponents { get; set; }
    public InfoResponse info { get; set; }
}

public class InfoResponse : Info
{
    public List<ParticipantResponse> participants { get; set; }
}

public class ParticipantResponse : Participant
{
    public List<Item> items { get; set; }
    public int cs  => totalMinionsKilled + neutralMinionsKilled;
    public double cSperMinute { get; set; }
    
    public double killParticipation { get; set; }
    public double kda => deaths == 0 ? -1 : (double)(kills + assists) / (double)deaths;
}

public class Item
{
    public Item(string id, string name, string description, string baseCost, string totalCost)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.baseCost = baseCost;
        this.totalCost = totalCost;
    }

    public string id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string baseCost { get; set; }
    public string totalCost { get; set; }
}

public class QueueType
{
    public int queueId { get; set; }
    public string map { get; set; }
    public string description { get; set; }
    public string notes { get; set; }
}