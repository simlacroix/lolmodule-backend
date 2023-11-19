using System.Net;
using LoLModule.Exceptions;
using LoLModule.Models;
using LoLModule.Models.Response;
using LoLModule.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace LoLModule.Services.Impl;

public class PlayerService : IPlayerService
{
    private readonly LoLApiClient _client;
    private readonly ILogger _logger;
    private readonly ISummonerRepository _summonerRepository;

    public PlayerService(ILogger<PlayerService> logger, ISummonerRepository summonerRepository)
    {
        _logger = logger;
        _client = new LoLApiClient();
        _summonerRepository = summonerRepository;
    }

    public async Task<string> checkIfSummonerExists(string name)
    {
        _logger.LogInformation($"Generating basic stats for summoner: {name} exists");

        var summoner = await getSummonerByName(name);

        if (summoner is null)
            throw new UserNotFoundException($"Summoner name doesn't exist {name}");
        
        _logger.LogDebug($"Response: {summoner.name}");
        return summoner.name;
    }

    public async Task<SummonerResponse?> getSummonerByName(string name)
    {
        _logger.LogInformation($"Getting summoner: {name}");
        SummonerResponse? summoner;
        summoner = await _summonerRepository.GetSummonerByName(name);

        if (summoner == null)
        {
            var callUrl = $"lol/summoner/v4/summoners/by-name/{name}?api_key={Globals.ApiKey}";
            var response = await _client.GetAsync(callUrl);
            while (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                Thread.Sleep(1000);
                response = await _client.GetAsync(callUrl);
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                summoner = JsonConvert.DeserializeObject<SummonerResponse>(content);
                await _summonerRepository.Create(summoner);
            }
            else if (response.StatusCode != HttpStatusCode.TooManyRequests &&
                     response.StatusCode != HttpStatusCode.NotFound)
            {
                throw new Exception($"Error {response.StatusCode}: {response.RequestMessage}");
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        _logger.LogDebug($"Response: {summoner}");
        return summoner;
    }
}