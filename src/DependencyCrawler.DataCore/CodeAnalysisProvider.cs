namespace DependencyCrawler.DataCore;

internal class CodeAnalysisProvider(IEnumerable<ICodeAnalysis> codeAnalyses) : ICodeAnalysisProvider
{
	public IReadOnlyList<ICodeAnalysis> CodeAnalyses => codeAnalyses.ToList();
}