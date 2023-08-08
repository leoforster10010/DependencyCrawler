namespace DependencyCrawler.Contracts.Interfaces.Repositories;

internal interface IDllFileProvider
{
	IEnumerable<string> GetDllFiles();
	string? GetDllFile(string projectName);
	bool GetIsExternal(string projectName);
}