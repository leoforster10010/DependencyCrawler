namespace DependencyCrawler.Data.Sqlite;

internal class SerializedDataCore
{
	public required Guid Id { get; init; }
	public required string Payload { get; init; }
}