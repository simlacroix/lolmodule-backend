namespace LoLModule.Models;

public class ChampionStats
{
    public int championId { get; set; }
    public string championName { get; set; }
    public double winRatio { get; set; }
    public double CS  { get; set; }
    public double CSPerMinute { get; set; }
    public double KDA { get; set; }
    public double averageKill { get; set; }
    public double averageDeath { get; set; }
    public double averageAssist { get; set; }
    public int gamesPlayed { get; set; }

    public ChampionStats(int championId, string championName, double winRatio, double cs, double csPerMinute, double kda, double averageKill, double averageDeath, double averageAssist, int gamesPlayed)
    {
        this.championId = championId;
        this.championName = championName;
        this.winRatio = winRatio;
        CS = cs;
        CSPerMinute = csPerMinute;
        KDA = kda;
        this.averageKill = averageKill;
        this.averageDeath = averageDeath;
        this.averageAssist = averageAssist;
        this.gamesPlayed = gamesPlayed;
    }
}