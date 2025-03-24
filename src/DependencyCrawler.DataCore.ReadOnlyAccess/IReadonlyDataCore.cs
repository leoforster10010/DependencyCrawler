using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore.ReadOnlyAccess;

public interface IReadOnlyDataCore : IValueDataCore
{
	bool IsActive { get; }
	IReadOnlyDictionary<string, IReadOnlyModule> ModulesReadOnly { get; }
}