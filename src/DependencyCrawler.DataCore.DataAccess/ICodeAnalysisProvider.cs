namespace DependencyCrawler.DataCore.DataAccess;

public interface ICodeAnalysisProvider
{
	IReadOnlyList<ICodeAnalysis> CodeAnalyses { get; }
}