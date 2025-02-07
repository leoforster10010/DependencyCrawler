namespace DependencyCrawler.DataCore.DataAccess;

public interface IEntity
{
	IDataCore DataCore { get; }
	IDataCoreProvider DataCoreProvider { get; }
	IReadOnlyList<IEntity> IngoingReferences { get; }
	IReadOnlyList<IEntity> OutgoingReferences { get; }
	void Delete();
}