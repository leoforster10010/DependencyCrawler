using DependencyCrawler.DataCore.ReadOnlyAccess;

namespace DependencyCrawler.DataCore;

public interface IDataCore : IReadOnlyDataCore
{
    IDataCoreProvider DataCoreProvider { get; }
    IReadOnlyDictionary<string, IModule> Modules { get; }
    IReadOnlyList<IEntity> Entities { get; }
    void Activate();
    void Delete();
    IModule GetOrCreateModule(string name);
}