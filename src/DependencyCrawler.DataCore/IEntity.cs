namespace DependencyCrawler.DataCore;

internal interface IEntity
{
	IDataCore DataCore { get; }
	DataCoreProvider DataCoreProvider { get; }
	IReadOnlyList<IEntity> IngoingReferences { get; }
	IReadOnlyList<IEntity> OutgoingReferences { get; }
	void Delete();
}