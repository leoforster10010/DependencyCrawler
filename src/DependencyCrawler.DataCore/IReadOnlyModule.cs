namespace DependencyCrawler.DataCore;

internal interface IReadOnlyModule
{
	string NameReadOnly { get; }
	IReadOnlyDictionary<string, IReadOnlyModule> DependingOnReadOnly { get; }
	IReadOnlyDictionary<string, IReadOnlyModule> DependencyOfReadOnly { get; }
}