using System.Collections.Concurrent;
using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ReadOnlyAccess;
using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore;

internal class DataCore : IDataCore, IReadonlyDataCore, IValueDataCore
{
	public bool IsActive => DataAccess.Core.Id == Id;
	public required IDataAccess DataAccess { get; init; }
	public Guid Id { get; } = Guid.NewGuid();
	public ConcurrentDictionary<string, Module> Modules { get; } = new();
	public IReadOnlyDictionary<string, IReadOnlyModule> ModulesReadOnly => Modules.ToDictionary(key => key.Key, value => value.Value as IReadOnlyModule);
	public IReadOnlyDictionary<string, IValueModule> ModulesValue => Modules.ToDictionary(key => key.Key, value => value.Value as IValueModule);
}