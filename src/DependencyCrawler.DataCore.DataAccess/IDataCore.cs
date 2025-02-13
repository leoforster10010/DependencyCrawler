using DependencyCrawler.DataCore.ReadOnlyAccess;

namespace DependencyCrawler.DataCore.DataAccess;

public interface IDataCore : IReadOnlyDataCore
{
	IDataCoreProvider DataCoreProvider { get; }
	IReadOnlyDictionary<string, IModule> Modules { get; }
	IReadOnlyList<IEntity> Entities { get; }
	bool IsEmpty => !Entities.Any();
	void Activate();
	void Delete();
	IModule GetOrCreateModule(string name);
}