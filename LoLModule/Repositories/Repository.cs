using LoLModule.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LoLModule.Repositories;

public abstract class Repository
{
    protected readonly IMongoDatabase MongoDatabase;

    protected Repository(IOptions<LoLDatabaseSettings> lolDatabaseSettings)
    {
        var mongoClient = new MongoClient(lolDatabaseSettings.Value.connectionString);
        MongoDatabase = mongoClient.GetDatabase(lolDatabaseSettings.Value.databaseName);
    }
}