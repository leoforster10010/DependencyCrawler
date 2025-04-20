namespace DependencyCrawler.Data.Postgresql;

public class SerializedDataCore
{
	public required Guid Id { get; init; }
	public required string Payload { get; init; }
}