using DependencyCrawler.DataCore.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyCrawler.Data.Sqlite;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddSqliteDataSource(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<IDataSource, SqliteDataSource>();
		services.AddOptionsWithValidateOnStart<DataSqliteSettings>()
			.BindConfiguration(nameof(DataSqliteSettings))
			.ValidateDataAnnotations();
		services.AddDbContext<DependencyCrawlerContext>(options =>
			options.UseSqlite($"Data Source={configuration.GetSection(nameof(DataSqliteSettings)).Get<DataSqliteSettings>()!.DbPath}"));

		return services;
	}
}