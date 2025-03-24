using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Data.Contracts.Enum;
using DependencyCrawler.Framework.Extensions;
using Microsoft.Extensions.Configuration;

namespace DependencyCrawler.Implementations.Repositories.Provider;

internal class DllFileProvider : IDllFileProvider
{
	private const string Extension = "*.dll";
	private readonly IConfiguration _configuration;
	private List<string>? _dllFiles;

	public DllFileProvider(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public IEnumerable<string> GetDllFiles()
	{
		if (_dllFiles is not null)
		{
			return _dllFiles;
		}

		var path = _configuration[ConfigurationKeys.DllDirectory.ToString()];
		_dllFiles = Directory.GetFiles(path!, Extension, SearchOption.AllDirectories).ToList();

		return _dllFiles;
	}

	public string? GetDllFile(string projectName)
	{
		return GetDllFiles().FirstOrDefault(x => x.GetDllName() == projectName);
	}

	public bool GetIsExternal(string projectName)
	{
		return GetDllFiles().Any(x => x.GetDllName() == projectName);
	}
}