using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.Framework;

internal class EventLoggerProvider(IConfiguration configuration, IEventLogProvider eventLogProvider) : ILoggerProvider
{
	public void Dispose()
	{
	}

	public ILogger CreateLogger(string categoryName)
	{
		return new EventLogger(configuration, eventLogProvider, categoryName);
	}
}