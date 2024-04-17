namespace DependencyCrawler.DataCore;

internal interface IDependencyCrawlerReadonlyCore
{
	IReadOnlyDictionary<string, IReadOnlyModule> ModulesReadOnly { get; }
	Guid Id { get; }
}