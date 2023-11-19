using System.Net;
using LoLModule.Models;
using LoLModule.Models.Response;
using LoLModule.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoLModule.Services.Impl;

public class MatchService : IMatchService
{
    private readonly HttpClient _client;
    private readonly LoLApiClient _clientNa1;
    private readonly ILogger _logger;
    private readonly IMatchRepository _matchRepository;
    private JObject _itemContent;

    public MatchService(ILogger<MatchService> logger, IMatchRepository matchRepository)
    {
        _logger = logger;
        _client = new HttpClient();
        _client.BaseAddress = new Uri(Globals.BaseAmericaUrl);
        _clientNa1 = new LoLApiClient();
        _matchRepository = matchRepository;
    }

    public async Task<BasicStats> generateBasicStats(SummonerResponse summoner)
    {
        _logger.LogInformation($"Generating basic stats for summoner: {summoner.name} matches");
        var matches = await getMatchesFromSummoner(summoner);
        matches.Sort((x, y) => y.info.gameEndTimestamp.CompareTo(x.info.gameEndTimestamp));

        await setItemContent();
        List<LeagueEntryResponse> leagueEntry =  await getPlayerLeagueEntry(summoner);
        List<MatchResponse> infos = generateMatchInfoListFromMatches(matches, summoner);

        var response = new BasicStats(summoner, leagueEntry, summoner.name, summoner.profileIconId,getMostPlayedPosition(infos), generateChampionStats(infos), infos);
        _logger.LogDebug($"Response: {response}");
        return response;
    }

    public async Task<List<MatchResponse>> getMatchesFromSummoner(SummonerResponse summoner)
    {
        var matchIds = await getMatchIdsFromSummonerPuuid(summoner?.puuid);
        var matches = await _matchRepository.GetAllBySummoner(summoner.id);
        
        foreach (var id in matchIds)
            if (!matches.Where(x => x.Id == id).Any())
            {
                var match = await getMatchFromId(id);
                match.Id = id;
                await _matchRepository.Create(match);
                matches.Add(match);
            }

        return matches;
    }

    public async Task<MatchResponse> getMatchFromId(string matchId)
    {
        _logger.LogInformation($"Getting match from id: {matchId}");

        var callUrl = $"/lol/match/v5/matches/{matchId}?api_key={Globals.ApiKey}";
        var response = await _client.GetAsync(callUrl);

        while (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            Thread.Sleep(1000);
            response = await _client.GetAsync(callUrl);
        }

        var match = new MatchResponse();

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            match = JsonConvert.DeserializeObject<MatchResponse>(content);
        }
        else
        {
            throw new Exception($"Error {response.StatusCode}: {response.RequestMessage}");
        }

        _logger.LogDebug($"Response: {match}");
        return match;
    }

    public async Task<List<string>> getMatchIdsFromSummonerPuuid(string puuid)
    {
        _logger.LogInformation($"Getting match ids from summonerId: {puuid}");

        var callUrl = $"/lol/match/v5/matches/by-puuid/{puuid}/ids?count=50&api_key={Globals.ApiKey}";
        var response = await _client.GetAsync(callUrl);

        var matches = new List<string>();
        while (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            Thread.Sleep(1000);
            response = await _client.GetAsync(callUrl);
        }

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            matches = JsonConvert.DeserializeObject<List<string>>(content);
        }
        else
        {
            throw new Exception($"Error {response.StatusCode}: {response.RequestMessage}");
        }

        _logger.LogDebug($"Response: {matches}");
        return matches;
    }

    public double getPlayerWinRatioWithChampion(List<MatchResponse> matches, string championName, string puuid)
    {
        _logger.LogInformation($"Getting win ratio for champion: {championName}");
        var winCount = 0;
        var gamesWithChamp = 0;
        foreach (var match in matches)
        {
            var focusedPlayer = match.focusedPlayer != null
                ? match.focusedPlayer
                : match.info.participants.Find(p => p.puuid == puuid);
            if (focusedPlayer.championName == championName)
            {
                if (focusedPlayer.win)
                    winCount++;
                gamesWithChamp++;
            }
        }

        return gamesWithChamp != 0 ? winCount / (double)gamesWithChamp : 0;
    }

    public double getPlayerWinRatioWithPosition(List<MatchResponse> matches, string lane, string puuid)
    {
        _logger.LogInformation($"Getting player win ratio with position: {lane}");

        var winCount = 0;
        var gamesOnLane = 0;
        foreach (var match in matches)
        {
            var focusedPlayer = match.focusedPlayer != null
                ? match.focusedPlayer
                : match.info.participants.Find(p => p.puuid == puuid);

            if (focusedPlayer.lane == lane)
            {
                if (focusedPlayer.win)
                    winCount++;
                gamesOnLane++;
            }
        }

        return gamesOnLane != 0 ? winCount / (double)gamesOnLane : 0;
    }

    public List<MatchResponse> generateMatchInfoListFromMatches(List<MatchResponse> matches, SummonerResponse summoner)
    {
        _logger.LogInformation($"Generate match info list from matches, for summoner: {summoner.name}");
        foreach (var match in matches)
        {
            var player = match.info.participants.Find(p => p.puuid == summoner.puuid);

            match.focusedPlayer = player;
            match.teammates = new TeamResponse(match.info.participants.Where(p => p.teamId == player.teamId).ToList());
            match.opponents = new TeamResponse(match.info.participants.Where(p => p.teamId != player.teamId).ToList());

            CalculateTeam(match.teammates, match.info.gameDuration);
            CalculateTeam(match.opponents, match.info.gameDuration);

            match.info.participants = null;
        }

        _logger.LogDebug($"Response: {matches}");
        return matches;
    }

    public string getMostPlayedPosition(List<MatchResponse> matches)
    {
        _logger.LogInformation("Getting most played position");

        var response = (from x in matches
            group x by x.focusedPlayer.lane
            into lane
            orderby lane.Count() descending
            select lane.Key).First();

        _logger.LogDebug($"Response: {matches}");
        return response;
    }

    public async Task<List<LeagueEntryResponse>?> getPlayerLeagueEntry(SummonerResponse summoner)
    {
        _logger.LogInformation($"Getting summoner: {summoner.name} League Entry");
        var callUrl = $"/lol/league/v4/entries/by-summoner/{summoner.id}?api_key={Globals.ApiKey}";
        var leagueEntry = new List<LeagueEntryResponse>();
        ;
        var response = await _clientNa1.GetAsync(callUrl);
        while (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            Thread.Sleep(1000);
            response = await _clientNa1.GetAsync(callUrl);
        }

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            leagueEntry = JsonConvert.DeserializeObject<List<LeagueEntryResponse>>(content);
        }
        else
        {
            leagueEntry.Add(new LeagueEntryResponse());
        }

        _logger.LogDebug($"Response: {leagueEntry}");
        return leagueEntry;
    }

    public double getPlayerWinRatio(LeagueEntryDto leagueEntryDto)
    {
        _logger.LogInformation($"Getting win ratio: {leagueEntryDto}");

        var totalGames = leagueEntryDto.wins + (double)leagueEntryDto.losses;
        var response = totalGames != 0 ? leagueEntryDto.wins / totalGames : 0;

        _logger.LogDebug($"Response: {response}");
        return response;
    }

    private void CalculateTeam(TeamResponse team, int gameDuration)
    {
        foreach (var participant in team.teammates)
        {
            participant.cSperMinute = participant.cs / ((double)gameDuration / 60);

            participant.items = getPlayerItems(new List<string>
            {
                participant.item0.ToString(), participant.item1.ToString(), participant.item2.ToString(),
                participant.item3.ToString(), participant.item4.ToString(), participant.item5.ToString(),
                participant.item6.ToString()
            });

            participant.killParticipation = team.totalKills == 0
                ? 0
                : (double)(participant.kills + participant.assists) / team.totalKills;
        }
    }

    private List<ChampionStats> generateChampionStats(List<MatchResponse> matches)
    {
        var championStatsList = new List<ChampionStats>();
        var champGroups = from x in matches
            group x by x.focusedPlayer.championName
            into champs
            orderby champs.Count() descending
            select champs;
        foreach (var group in champGroups)
        {
            var championId = group.First().focusedPlayer.championId;
            var championName = group.Key;
            var gamesPlayed = group.Count();

            double winRatio;

            double cs = 0;
            double csPerMinute = 0;
            double kda = 0;
            double averageKill = 0;
            double averageDeath = 0;
            double averageAssist = 0;

            double win = 0;

            foreach (var match in group)
            {
                cs += match.focusedPlayer.cs;
                csPerMinute += match.focusedPlayer.cSperMinute;
                if (match.focusedPlayer.deaths > 0)
                    kda += (match.focusedPlayer.kills + match.focusedPlayer.assists) /
                           (double)match.focusedPlayer.deaths;
                averageKill += match.focusedPlayer.kills;
                averageDeath += match.focusedPlayer.deaths;
                averageAssist += match.focusedPlayer.assists;
                if (match.focusedPlayer.win)
                    win++;
            }

            cs /= gamesPlayed;
            csPerMinute /= gamesPlayed;
            kda /= gamesPlayed;
            averageKill /= gamesPlayed;
            averageDeath /= gamesPlayed;
            averageAssist /= gamesPlayed;
            winRatio = win / gamesPlayed;
            var champStats = new ChampionStats(championId, championName, winRatio, cs, csPerMinute, kda,
                averageKill, averageDeath, averageAssist, gamesPlayed);
            championStatsList.Add(champStats);
        }

        return championStatsList;
    }


    private async Task setItemContent()
    {
        var itemClient = new HttpClient();
        var itemResonse = await itemClient.GetAsync(Globals.ItemInfoUrl);
        var content = await itemResonse.Content.ReadAsStringAsync();
        _itemContent = JObject.Parse(content);
    }

    private List<Item> getPlayerItems(List<string> ids)
    {
        var items = new List<Item>();
        foreach (var id in ids)
            if (id != "0" && id != null)
                items.Add(new Item(id, _itemContent["data"]?[id]?["name"]?.ToString(),
                    _itemContent["data"]?[id]?["description"]?.ToString(),
                    _itemContent["data"]?[id]?["gold"]?["base"]?.ToString(),
                    _itemContent["data"]?[id]?["gold"]?["total"]?.ToString()));
            else
                items.Add(new Item(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

        return items;
    }
}