namespace DependencyCrawler.DataCore;

public interface ICodeAnalysisProvider
{
	IReadOnlyList<ICodeAnalysis> CodeAnalyses { get; }
}