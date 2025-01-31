namespace DependencyCrawler.DataCore;

public interface IDataSource
{
    Guid Id { get; }
    void Save();
    void Load();
}