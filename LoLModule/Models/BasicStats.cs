using LoLModule.Models.Response;

namespace LoLModule.Models;

public class BasicStats
{
    public BasicStats(SummonerResponse summoner, List<LeagueEntryResponse> leagueEntry, string playerName, int profileIconId, string mostPlayedPosition, List<ChampionStats> championStats, List<MatchResponse> matchHistory)
    {
        this.summoner = summoner;
        this.leagueEntry = leagueEntry.Where(x => x.queueType == "RANKED_FLEX_SR" || x.queueType == "RANKED_SOLO_5x5").ToList();
        this.playerName = playerName;
        this.profileIconId = profileIconId;
        this.mostPlayedPosition = mostPlayedPosition;
        this.championStats = championStats;
        this.matchHistory = matchHistory;
    }
    
    public SummonerResponse summoner { get; set; }
    public List<LeagueEntryResponse> leagueEntry { get; set; }
    public string playerName { get; set; }
    public int profileIconId { get; set; }
    public string mostPlayedPosition { get; set; }
    public List<ChampionStats> championStats { get; set; }
    public List<MatchResponse> matchHistory { get; set; }

}