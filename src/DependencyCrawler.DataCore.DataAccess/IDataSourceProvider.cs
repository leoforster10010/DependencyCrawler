namespace DependencyCrawler.DataCore;

public interface IDataSourceProvider
{
    IReadOnlyDictionary<Guid, IDataSource> DataSources { get; }
}