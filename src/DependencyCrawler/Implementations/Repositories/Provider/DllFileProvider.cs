using DependencyCrawler.Contracts.Interfaces;
using DependencyCrawler.Framework.Extensions;

namespace DependencyCrawler.Implementations.Repositories.Provider;

public class DllFileProvider : IDllFileProvider
{
	private const string Extension = "*.dll";
	private readonly List<string> _dllFiles;

	private readonly string _nugetPath =
		Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget");

	public DllFileProvider()
	{
		_dllFiles = Directory.GetFiles(_nugetPath, Extension, SearchOption.AllDirectories).ToList();
	}

	public IEnumerable<string> GetDllFiles()
	{
		return _dllFiles;
	}

	public string? GetDllFile(string projectName)
	{
		//ToDo test
		return _dllFiles.FirstOrDefault(x => x.GetDllName() == projectName);
	}

	public bool GetIsExternal(string projectName)
	{
		return _dllFiles.Any(x => x.GetDllName() == projectName);
	}
}