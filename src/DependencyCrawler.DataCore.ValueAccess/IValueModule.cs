using DependencyCrawler.Data.Contracts;

namespace DependencyCrawler.DataCore.ValueAccess;

public interface IValueModule
{
	string Name { get; }
	HashSet<string> DependenciesValue { get; }
	HashSet<string> ReferencesValue { get; }
	ModuleType Type { get; }
}