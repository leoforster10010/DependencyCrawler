namespace DependencyCrawler.DataCore.ReadOnlyAccess;

public interface IDependencyCrawlerReadonlyCore
{
	IReadOnlyDictionary<string, IReadOnlyModule> ModulesReadOnly { get; }
	Guid Id { get; }
}