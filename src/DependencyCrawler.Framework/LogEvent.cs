using Microsoft.Extensions.Logging;

namespace DependencyCrawler.Framework;

public class LogEvent
{
	public required LogLevel LogLevel { get; init; }
	public required EventId EventId { get; init; }
	public required string CategoryName { get; init; }
	public required string Message { get; init; }
}