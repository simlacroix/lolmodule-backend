using LoLModule.Models.Response;

namespace LoLModule.Repositories;

public interface ISummonerRepository
{
    public Task<SummonerResponse?> GetSummonerByName(string summonerName);
    public Task Create(SummonerResponse summonerResponse);
}