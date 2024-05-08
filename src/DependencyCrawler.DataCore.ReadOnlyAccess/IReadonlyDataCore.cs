namespace DependencyCrawler.DataCore.ReadOnlyAccess;

public interface IReadonlyDataCore
{
	IReadOnlyDictionary<string, IReadOnlyModule> ModulesReadOnly { get; }
	Guid Id { get; }
}