namespace DependencyCrawler.DataCore;

internal interface IModule : IEntity
{
	string Name { get; }
	IReadOnlyDictionary<string, IModule> Dependencies { get; }
	IReadOnlyDictionary<string, IModule> References { get; }
	int DependencyLayer { get; }
	int ReferenceLayer { get; }
	bool IsTopLevel { get; }
	bool IsSubLevel { get; }
	void AddDependency(IModule module);
	void AddReference(IModule module);
	void RemoveDependency(IModule module);
	void RemoveReference(IModule module);
}