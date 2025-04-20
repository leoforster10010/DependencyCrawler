using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace DependencyCrawler.Data.MongoDB;

internal class MongoDbClientProvider(IConfiguration configuration, IMongoClient mongoClient) : IMongoDbClientProvider
{
	private IMongoDatabase? _dependencyCrawlerDatabase;
	private IMongoCollection<SerializedDataCore>? _serializedDataCoreCollection;

	public IMongoDatabase DependencyCrawlerDatabase => _dependencyCrawlerDatabase ??= mongoClient.GetDatabase(configuration.GetSection(nameof(DataMongoDbSettings)).Get<DataMongoDbSettings>()!.DatabaseName);
	public IMongoCollection<SerializedDataCore> SerializedDataCoreCollection => _serializedDataCoreCollection ??= DependencyCrawlerDatabase.GetCollection<SerializedDataCore>(nameof(SerializedDataCore));
}