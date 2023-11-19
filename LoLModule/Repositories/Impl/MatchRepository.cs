using LoLModule.Models;
using LoLModule.Models.Response;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LoLModule.Repositories.Impl;

public class MatchRepository : Repository, IMatchRepository
{
    private readonly IMongoCollection<MatchResponse> _matchCollection;

    public MatchRepository(IOptions<LoLDatabaseSettings> lolDatabaseSettings) : base(lolDatabaseSettings)
    {
        _matchCollection =
            MongoDatabase.GetCollection<MatchResponse>(lolDatabaseSettings.Value.lolMatchesCollectionName);

    }
    
    public async Task Create(MatchResponse match)
    {
        await _matchCollection.InsertOneAsync(match);
    }

    public async Task<List<MatchResponse>> GetAllBySummoner(string summonerId)
    {
        return _matchCollection.Find(x => x.info.participants.Any(y => y.summonerId == summonerId)).ToList();
    }
}