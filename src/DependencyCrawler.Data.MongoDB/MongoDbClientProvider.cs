using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace DependencyCrawler.Data.MongoDB;

internal class MongoDbClientProvider(IConfiguration configuration) : IMongoDbClientProvider
{
	private IMongoDatabase? _dependencyCrawlerDatabase;
	private IMongoClient? _mongoClient;
	private IMongoCollection<SerializedDataCore>? _serializedDataCoreCollection;

	public IMongoClient MongoClient => _mongoClient ??= new MongoClient(configuration.GetSection(nameof(DataMongoDbSettings)).Get<DataMongoDbSettings>()!.ConnectionString);
	public IMongoDatabase DependencyCrawlerDatabase => _dependencyCrawlerDatabase ??= MongoClient.GetDatabase(configuration.GetSection(nameof(DataMongoDbSettings)).Get<DataMongoDbSettings>()!.DatabaseName);
	public IMongoCollection<SerializedDataCore> SerializedDataCoreCollection => _serializedDataCoreCollection ??= DependencyCrawlerDatabase.GetCollection<SerializedDataCore>(nameof(SerializedDataCore));
}