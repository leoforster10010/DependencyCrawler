namespace DependencyCrawler.DataCore;

internal interface IValueModule
{
	string NameValue { get; }
	Dictionary<string, string> DependingOnValue { get; }
	Dictionary<string, string> DependencyOfValue { get; }
}