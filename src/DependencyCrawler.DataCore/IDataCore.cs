namespace DependencyCrawler.DataCore;

public interface IDataCore
{
	Guid Id { get; }
	bool IsActive { get; }
	IReadOnlyDictionary<string, IModule> Modules { get; }
	IReadOnlyList<IEntity> Entities { get; }
	void Activate();
	void Delete();
	IModule CreateModule(string name);
}