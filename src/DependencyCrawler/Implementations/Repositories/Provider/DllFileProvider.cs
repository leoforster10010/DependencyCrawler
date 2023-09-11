using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Framework.Extensions;
using DependencyCrawler.Implementations.Data.Enum;
using Microsoft.Extensions.Configuration;

namespace DependencyCrawler.Implementations.Repositories.Provider;

internal class DllFileProvider : IDllFileProvider
{
	private const string Extension = "*.dll";
	private readonly List<string> _dllFiles;

	public DllFileProvider(IConfiguration configuration)
	{
		var path = configuration[ConfigurationKeys.DllDirectory.ToString()];
		_dllFiles = Directory.GetFiles(path!, Extension, SearchOption.AllDirectories).ToList();
	}

	public IEnumerable<string> GetDllFiles()
	{
		return _dllFiles;
	}

	public string? GetDllFile(string projectName)
	{
		return _dllFiles.FirstOrDefault(x => x.GetDllName() == projectName);
	}

	public bool GetIsExternal(string projectName)
	{
		return _dllFiles.Any(x => x.GetDllName() == projectName);
	}
}