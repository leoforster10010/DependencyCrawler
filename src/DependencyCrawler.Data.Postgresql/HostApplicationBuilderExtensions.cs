using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DependencyCrawler.Data.Postgresql;

public static class HostApplicationBuilderExtensions
{
	public static IHostApplicationBuilder AddPostgresqlDataSource(this IHostApplicationBuilder builder)
	{
		builder.Services.AddScoped<IDataSource, PostgresqlDataSource>();

		builder.AddNpgsqlDbContext<DependencyCrawlerContext>(Constants.PostgresdbName);
		return builder;
	}
}