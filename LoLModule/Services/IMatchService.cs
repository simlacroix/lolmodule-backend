using System.Text.RegularExpressions;
using LoLModule.Models;
using LoLModule.Models.Response;

namespace LoLModule.Services;

public interface IMatchService
{
    public Task<BasicStats> generateBasicStats(SummonerResponse summoner);
    public Task<MatchResponse> getMatchFromId(string matchId);
    public Task<List<string>> getMatchIdsFromSummonerPuuid(string puuid);
    public double getPlayerWinRatioWithChampion(List<MatchResponse> matches, string championName, string puuid);
    public double getPlayerWinRatioWithPosition(List<MatchResponse> matches, string lane, string puuid);
    public Task<List<MatchResponse>> getMatchesFromSummoner(SummonerResponse summoner);
    protected internal List<MatchResponse> generateMatchInfoListFromMatches(List<MatchResponse> matches, SummonerResponse summoner);
    protected internal string getMostPlayedPosition(List<MatchResponse> matches);
    public Task<List<LeagueEntryResponse>?> getPlayerLeagueEntry(SummonerResponse summoner);
    protected internal double getPlayerWinRatio(LeagueEntryDto leagueEntryDto);
}