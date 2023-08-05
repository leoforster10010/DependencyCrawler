namespace DependencyCrawler.Contracts.Interfaces.Repositories;

public interface IProjectFileProvider
{
	IEnumerable<string> GetProjectFiles();
	string? GetProjectFile(string projectName);
	string? GetProjectDirectory(string projectName);
	bool GetIsInternal(string projectName);
}