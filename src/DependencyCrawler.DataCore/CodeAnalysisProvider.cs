namespace DependencyCrawler.DataCore;

public class CodeAnalysisProvider(IEnumerable<ICodeAnalysis> codeAnalyses)
{
	public IReadOnlyList<ICodeAnalysis> CodeAnalyses => codeAnalyses.ToList();
}