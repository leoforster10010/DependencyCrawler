namespace DependencyCrawler.DataCore.DataAccess;

public interface IDataSourceProvider
{
	IReadOnlyDictionary<Guid, IDataSource> DataSources { get; }
}