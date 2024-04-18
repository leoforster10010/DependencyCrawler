namespace DependencyCrawler.DataCore.ValueAccess;

public interface IDependencyCrawlerValueCore
{
	IReadOnlyDictionary<string, IValueModule> ModulesValue { get; }
	Guid Id { get; }
}