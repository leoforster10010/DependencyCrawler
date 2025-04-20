namespace DependencyCrawler.DataCore.DataAccess;

public interface IDataDiscovery
{
	public Guid Id { get; }
	public string Name { get; }

	public Task Load(string? filePath = null);
}