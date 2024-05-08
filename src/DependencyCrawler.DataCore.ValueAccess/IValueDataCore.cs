namespace DependencyCrawler.DataCore.ValueAccess;

public interface IValueDataCore
{
	IReadOnlyDictionary<string, IValueModule> ModulesValue { get; }
	Guid Id { get; }
}