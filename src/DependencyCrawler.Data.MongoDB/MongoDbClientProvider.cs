using DependencyCrawler.DataCore.ValueAccess;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace DependencyCrawler.Data.MongoDB;

internal class MongoDbClientProvider(IConfiguration configuration) : IMongoDbClientProvider
{
	private IMongoCollection<DataCoreDTO>? _dataCoreDtocCollection;
	private IMongoDatabase? _dependencyCrawlerDatabase;
	private IMongoClient? _mongoClient;

	public IMongoClient MongoClient => _mongoClient ??= new MongoClient(configuration.GetSection(nameof(DataMongoDbSettings)).Get<DataMongoDbSettings>()!.ConnectionString);
	public IMongoDatabase DependencyCrawlerDatabase => _dependencyCrawlerDatabase ??= MongoClient.GetDatabase(configuration.GetSection(nameof(DataMongoDbSettings)).Get<DataMongoDbSettings>()!.DatabaseName);
	public IMongoCollection<DataCoreDTO> DataCoreDtocCollection => _dataCoreDtocCollection ??= DependencyCrawlerDatabase.GetCollection<DataCoreDTO>(nameof(DataCoreDTO));
}