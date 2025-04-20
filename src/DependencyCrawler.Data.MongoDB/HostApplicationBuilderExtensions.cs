using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DependencyCrawler.Data.MongoDB;

public static class HostApplicationBuilderExtensions
{
	public static IHostApplicationBuilder AddMongoDbDataSource(this IHostApplicationBuilder builder)
	{
		builder.Services.AddScoped<IDataSource, MongoDbDataSource>();
		builder.Services.AddScoped<IMongoDbClientProvider, MongoDbClientProvider>();
		builder.Services.AddOptionsWithValidateOnStart<DataMongoDbSettings>()
			.BindConfiguration(nameof(DataMongoDbSettings))
			.ValidateDataAnnotations();

		builder.AddMongoDBClient(Constants.MongoDbName);
		return builder;
	}
}