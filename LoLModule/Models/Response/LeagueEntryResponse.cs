namespace LoLModule.Models.Response;

public class LeagueEntryResponse : LeagueEntryDto
{
    public double winRatio =>  wins + losses != 0 ? (double)wins / ((double)wins + (double)losses) : 0;
}