namespace DependencyCrawler.DataCore.DataAccess;

public interface ICodeAnalysis
{
	public Task Load(string? filePath = null);
}