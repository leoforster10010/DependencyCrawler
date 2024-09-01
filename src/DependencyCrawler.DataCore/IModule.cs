using DependencyCrawler.DataCore.ReadOnlyAccess;

namespace DependencyCrawler.DataCore;

public interface IModule : IReadOnlyModule, IEntity
{
	IReadOnlyDictionary<string, IModule> Dependencies { get; }
	IReadOnlyDictionary<string, IModule> References { get; }
	void AddDependency(IModule module);
	void AddReference(IModule module);
	void RemoveDependency(IModule module);
	void RemoveReference(IModule module);
}