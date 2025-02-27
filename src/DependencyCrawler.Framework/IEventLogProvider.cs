namespace DependencyCrawler.Framework;

public interface IEventLogProvider
{
	public event EventHandler<LogEvent>? OnLogEvent;
	internal void Log(LogEvent logEvent);
}