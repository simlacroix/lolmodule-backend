using LoLModule.Models;
using LoLModule.Models.Response;

namespace LoLModule.Services;

public interface IPlayerService
{
    public Task<string> checkIfSummonerExists(string name);

    public Task<SummonerResponse?> getSummonerByName(string name);
}