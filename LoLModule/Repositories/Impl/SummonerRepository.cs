using LoLModule.Models;
using LoLModule.Models.Response;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LoLModule.Repositories.Impl;

public class SummonerRepository : Repository, ISummonerRepository
{
    private readonly IMongoCollection<SummonerResponse> _playerAccountCollection;
    
    public SummonerRepository(IOptions<LoLDatabaseSettings> lolDatabaseSettings) : base(lolDatabaseSettings)
    {
        _playerAccountCollection =
            MongoDatabase.GetCollection<SummonerResponse>(lolDatabaseSettings.Value.summonerCollectionName);
    }

    
    public async Task<SummonerResponse?> GetSummonerByName(string summonerName)
    {
        return await _playerAccountCollection
            .Find(x => string.Equals(x.name, summonerName, StringComparison.CurrentCultureIgnoreCase))
            .FirstOrDefaultAsync();

    }

    public async Task Create(SummonerResponse summonerResponse)
    {
        await _playerAccountCollection.InsertOneAsync(summonerResponse);
    }
}