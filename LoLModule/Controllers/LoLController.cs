using LoLModule.Exceptions;
using LoLModule.Models;
using LoLModule.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LoLModule.Controllers;

[ApiController]
[Route("[controller]")]
public class LoLController : ControllerBase
{
    private readonly string _basicStatsKey = "basicStatsKey";
    private readonly ILogger _logger;
    private readonly IMatchService _matchService;
    private readonly IPlayerService _playerService;


    public LoLController(IMatchService matchService, IPlayerService playerService, ILogger<LoLController> logger)
    {
        _playerService = playerService;
        _logger = logger;
        _matchService = matchService;
    }

    [HttpGet("getStatsForPlayer")]
    public async Task<IActionResult> getStatsForPlayer(string summonerName)
    {
        try
        {
            var summoner = await _playerService.getSummonerByName(summonerName);
            
            if (summoner is null)
            {
                _logger.LogInformation("Summoner is null, throwing exception");
                throw new Exception();
            }

            var stats = await _matchService.generateBasicStats(summoner);
            var returnResponse = JsonConvert.SerializeObject(stats);
            return Ok(returnResponse);
        }
        catch (Exception e)
        {
            _logger.LogInformation(e, $"exception while getting stats for: {summonerName}");
            return BadRequest(e.Message);
        }
    }

    [HttpGet("getChampionWinRate")]
    public async Task<IActionResult> getChampionWinRate(string summonerName, string champName)
    {
        var summoner = await _playerService.getSummonerByName(summonerName);
        var matches = await _matchService.getMatchesFromSummoner(summoner);
        if (matches.Count > 0)
            return Ok(JsonConvert.SerializeObject(
                _matchService.getPlayerWinRatioWithChampion(matches, champName, summoner.puuid)));

        BasicStats? stats;
        var result = await getStatsForPlayer(summonerName);

        if (result.GetType() == typeof(BadRequestObjectResult)) return result;

        var okObject = result as OkObjectResult;
        stats = JsonConvert.DeserializeObject<BasicStats>((string)okObject.Value);
        return Ok(JsonConvert.SerializeObject(
            _matchService.getPlayerWinRatioWithChampion(stats.matchHistory, champName, summoner.puuid)));
    }

    [HttpGet("getLaneWinRate")]
    public async Task<IActionResult> getLaneWinRate(string summonerName, string lane)
    {
        var summoner = await _playerService.getSummonerByName(summonerName);
        var matches = await _matchService.getMatchesFromSummoner(summoner);
        if (matches.Count > 0)
            return Ok(JsonConvert.SerializeObject(
                _matchService.getPlayerWinRatioWithPosition(matches, lane, summoner.puuid)));

        BasicStats? stats;
        var result = await getStatsForPlayer(summonerName);

        if (result.GetType() == typeof(BadRequestObjectResult)) return result;

        var okObject = result as OkObjectResult;
        stats = JsonConvert.DeserializeObject<BasicStats>((string)okObject.Value);
        return Ok(JsonConvert.SerializeObject(
            _matchService.getPlayerWinRatioWithPosition(stats.matchHistory, lane, summoner.puuid)));
    }

    [HttpGet("userExists")]
    public async Task<IActionResult> userExists(string summonerName)
    {
        try
        {
            var gamertag = await _playerService.checkIfSummonerExists(summonerName);
            return Ok(gamertag);
        }
        catch (UserNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogInformation(e, $"exception while verifying that summoner: {summonerName} exists");
            return BadRequest(e.Message);
        }
    }
}