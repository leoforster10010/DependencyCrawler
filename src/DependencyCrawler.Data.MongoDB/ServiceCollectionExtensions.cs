using DependencyCrawler.DataCore.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace DependencyCrawler.Data.MongoDB;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddMongoDbDataSource(this IServiceCollection services)
	{
		services.AddSingleton<IDataSource, MongoDbDataSource>();
		services.AddSingleton<IMongoDbClientProvider, MongoDbClientProvider>();
		services.AddOptionsWithValidateOnStart<DataMongoDbSettings>()
			.BindConfiguration(nameof(DataMongoDbSettings))
			.ValidateDataAnnotations();

		BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

		return services;
	}
}