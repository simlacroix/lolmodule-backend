using LoLModule.Models.Response;

namespace LoLModule.Repositories;

public interface IMatchRepository
{
    public Task Create(MatchResponse match);
    public Task<List<MatchResponse>> GetAllBySummoner(string summonerId);

}