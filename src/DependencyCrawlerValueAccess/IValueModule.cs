namespace DependencyCrawler.DataCore.ValueAccess;

public interface IValueModule
{
	string NameValue { get; }
	HashSet<string> DependenciesValue { get; }
	HashSet<string> ReferencesValue { get; }
}