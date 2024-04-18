namespace DependencyCrawler.DataCore.ValueAccess;

public interface IValueModule
{
	string NameValue { get; }
	HashSet<string> DependingOnValue { get; }
	HashSet<string> DependencyOfValue { get; }
}