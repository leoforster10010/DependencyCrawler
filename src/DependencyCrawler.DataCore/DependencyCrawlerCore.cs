using System.Collections.Concurrent;
using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ReadOnlyAccess;
using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore;

internal class DependencyCrawlerCore : IDependencyCrawlerCore, IDependencyCrawlerReadonlyCore, IDependencyCrawlerValueCore
{
	public bool IsActive => DependencyCrawlerDataAccess.Core.Id == Id;
	public Guid Id { get; } = Guid.NewGuid();
	public required IDependencyCrawlerDataAccess DependencyCrawlerDataAccess { get; init; }
	public ConcurrentDictionary<string, Module> Modules { get; } = new();
	public IReadOnlyDictionary<string, IReadOnlyModule> ModulesReadOnly => Modules.ToDictionary(key => key.Key, value => value.Value as IReadOnlyModule);
	public IReadOnlyDictionary<string, IValueModule> ModulesValue => Modules.ToDictionary(key => key.Key, value => value.Value as IValueModule);
}