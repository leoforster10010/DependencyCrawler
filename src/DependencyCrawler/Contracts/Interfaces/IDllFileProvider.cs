namespace DependencyCrawler.Contracts.Interfaces;

public interface IDllFileProvider
{
	IEnumerable<string> GetDllFiles();
	string? GetDllFile(string projectName);
	bool GetIsExternal(string projectName);
}