using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.Framework;

internal class EventLogger(IConfiguration configuration, IEventLogProvider eventLogProvider, string categoryName) : ILogger
{
	private readonly LogLevel _defaultLogLevel = Enum.TryParse(configuration["Logging:LogLevel:Default"], out LogLevel logLevel) ? logLevel : LogLevel.Information;

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		if (!IsEnabled(logLevel))
		{
			return;
		}

		eventLogProvider.Log(new LogEvent
		{
			LogLevel = logLevel,
			EventId = eventId,
			CategoryName = categoryName,
			Message = formatter(state, exception)
		});
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		return logLevel >= _defaultLogLevel;
	}

	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
	{
		return null;
	}
}