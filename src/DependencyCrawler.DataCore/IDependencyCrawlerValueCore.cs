namespace DependencyCrawler.DataCore;

internal interface IDependencyCrawlerValueCore
{
	IReadOnlyDictionary<string, IValueModule> ModulesValue { get; }
	Guid Id { get; }
}