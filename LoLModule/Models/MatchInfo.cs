using System.Linq.Expressions;
using LoLModule.Models.Dto;
using LoLModule.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoLModule.Models;

public class MatchInfo
{
    public string championName { get; set; }
    /*public int championId { get; set; }*/
    public bool win { get; set; }
    /*public int kills { get; set; }
    public int deaths { get; set; }
    public int assists { get; set; }*/
    public string lane { get; set; }
    /*public string role { get; set; }*/
    /*public List<Item> items { get; set; }
    public int team { get; set; }
    public List<Player> teammates { get; set; }
    public List<Player> opponents { get; set; }*/
    
    /*public int? controlWard { get; set; }
    public double? killParticipation { get; set; }*/
    public int CS { get; set; }
    public double CSperMinute { get; set; }

    public Rank averageRank { get; set; }
    public string gameType { get; set; }
    /*public int spell1Casts { get; set; }
    public int spell2Casts { get; set; }
    public int spell3Casts { get; set; }
    public int spell4Casts { get; set; }
    public int summoner1Casts { get; set; }
    public int summoner1Id { get; set; }
    public int summoner2Casts { get; set; }
    public int summoner2Id { get; set; }
    */
    


    //take a match directly from api to create a match object with the information we want
    public MatchInfo(MatchDto matchDto, string puuid)
    {
        Participant player = matchDto.info.participants.Find(p => p.puuid == puuid);
        /*championName = player.championName;
        championId = player.championId;
        win = player.win;
        kills = player.kills;
        deaths = player.deaths;
        assists = player.assists;
        lane = player.lane;
        role = player.role;
        team = player.teamId;*/
        //teammates = participantToPlayer(matchDto.info.participants.Where(p => p.teamId == team).ToList());
        //opponents = participantToPlayer(matchDto.info.participants.Where(p => p.teamId != team).ToList());
        /*if (player.challenges != null)
        {
            controlWard = player.challenges.controlWardsPlaced;
            killParticipation = player.challenges.killParticipation;   
        }*/
        CS = player.totalMinionsKilled + player.neutralMinionsKilled;
        CSperMinute = (double)CS / (double)((double)matchDto.info.gameDuration / 60);
        /*spell1Casts = player.spell1Casts;
        spell2Casts = player.spell2Casts;
        spell3Casts = player.spell3Casts;
        spell4Casts = player.spell4Casts;
        summoner1Casts = player.summoner1Casts;
        summoner1Id = player.summoner1Id;
        summoner2Casts = player.summoner2Casts;
        summoner2Id = player.summoner2Id;*/
    }

    /*private List<Player> participantToPlayer(List<Participant> participants)
    {
        List<Player> players = new List<Player>();
        foreach (Participant participant in participants)
        {
            players.Add(new Player(participant));
        }

        return players;
    }*/
    
}


/*public class Item
{
    public Item(string id, string name, string description)
    {
        this.id = id;
        this.name = name;
        this.description = description;
    }

    public string id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
}

public class QueueType
{
    public int queueId { get; set; }
    public string map { get; set; }
    public string description { get; set; }
    public string notes { get; set; }
}

public class Player
{
    public Player(Participant participant)
    {
        summonerName = participant.summonerName;
        championName = participant.championName;
        championId = participant.championId;
    }

    public string summonerName { get; set; }
    public string championName { get; set; }
    public int championId { get; set; }
    
    
}*/