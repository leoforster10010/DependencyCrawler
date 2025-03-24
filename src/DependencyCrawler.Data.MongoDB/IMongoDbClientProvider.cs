using MongoDB.Driver;

namespace DependencyCrawler.Data.MongoDB;

internal interface IMongoDbClientProvider
{
	IMongoClient MongoClient { get; }
	IMongoDatabase DependencyCrawlerDatabase { get; }
	IMongoCollection<SerializedDataCore> SerializedDataCoreCollection { get; }
}