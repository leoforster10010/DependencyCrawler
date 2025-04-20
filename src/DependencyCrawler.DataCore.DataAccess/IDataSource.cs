namespace DependencyCrawler.DataCore.DataAccess;

public interface IDataSource
{
	public Guid Id { get; }
	public string Name { get; }

	public Task Save();
	public Task Load();
}