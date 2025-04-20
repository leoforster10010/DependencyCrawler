using MongoDB.Driver;

namespace DependencyCrawler.Data.MongoDB;

internal interface IMongoDbClientProvider
{
	IMongoDatabase DependencyCrawlerDatabase { get; }
	IMongoCollection<SerializedDataCore> SerializedDataCoreCollection { get; }
}