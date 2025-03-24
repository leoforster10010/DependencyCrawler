namespace DependencyCrawler.Framework;

internal class EventLogProvider : IEventLogProvider
{
	public event EventHandler<LogEvent>? OnLogEvent;

	public void Log(LogEvent logEvent)
	{
		OnLogEvent?.Invoke(this, logEvent);
	}
}