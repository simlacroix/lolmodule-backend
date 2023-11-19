namespace LoLModule.Models;

public class LoLDatabaseSettings
{
    public string connectionString { get; set; } = null!;
    public string databaseName { get; set; } = null!;
    public string lolMatchesCollectionName { get; set; } = null!;
    public string summonerCollectionName { get; set; } = null!;

}