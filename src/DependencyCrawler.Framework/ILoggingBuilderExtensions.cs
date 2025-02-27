using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.Framework;

public static class ILoggingBuilderExtensions
{
	public static ILoggingBuilder AddEventLogger(this ILoggingBuilder builder, IConfiguration configuration)
	{
		builder.Services.AddSingleton<IEventLogProvider, EventLogProvider>();
		builder.Services.AddSingleton<ILoggerProvider, EventLoggerProvider>();
		return builder;
	}
}