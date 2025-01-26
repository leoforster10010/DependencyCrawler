using System.Collections.Concurrent;
using System.Reflection;
using DependencyCrawler.DataCore.ReadOnlyAccess;
using DependencyCrawler.DataCore.ValueAccess;
using Microsoft.Build.Construction;

namespace DependencyCrawler.DataCore;

public partial class DataCoreProvider : IDataCoreProvider
{
	private readonly IDictionary<Guid, IDataCore> _dataCores = new ConcurrentDictionary<Guid, IDataCore>();

	public DataCoreProvider()
	{
		ActiveCore = new DataCore(this);
	}

	public DataCoreProvider(DataCoreDTO dataCoreDTO)
	{
		ActiveCore = GetOrCreateDataCore(dataCoreDTO);
	}

	public IDataCore ActiveCore { get; private set; }
	public IReadOnlyDataCore ActiveCoreReadOnly => ActiveCore;
	public IValueDataCore ActiveCoreValue => ActiveCore;
	public IReadOnlyDictionary<Guid, IDataCore> DataCores => _dataCores.AsReadOnly();

	public IDataCore CreateDataCore()
	{
		return new DataCore(this);
	}

	public IDataCore GetOrCreateDataCore(DataCoreDTO dataCoreDto)
	{
		var dataCore = GetOrCreateDataCore(dataCoreDto.Id);

		foreach (var valueModule in dataCoreDto.ModuleValues)
		{
			var module = dataCore.GetOrCreateModule(valueModule.Name);

			foreach (var dependency in valueModule.DependencyValues)
			{
				var dependencyModule = dataCore.GetOrCreateModule(dependency);
				module.AddDependency(dependencyModule);
			}

			foreach (var reference in valueModule.ReferenceValues)
			{
				var referenceModule = dataCore.GetOrCreateModule(reference);
				module.AddReference(referenceModule);
			}
		}

		return dataCore;
	}

	public IDataCore GetOrCreateDataCore(Guid id)
	{
		return _dataCores.ContainsKey(id) ? DataCores[id] : new DataCore(this, id);
	}
}

public class DataDiscovery(IDataCoreProvider dataCoreProvider)
{
	private const string Path = @"C:\Users\_\source";

	public void Load()
	{
		//find all project files
		//ToDo: _path + pattern to config
		var projectFiles = Directory.GetFiles(Path, "*.csproj", SearchOption.AllDirectories);

		var modules = new Dictionary<string, ModuleDTO>();

		//extract info to ModuleDTO
		foreach (var projectFile in projectFiles)
		{
			var projectRootElement = ProjectRootElement.Open(projectFile);

			if (projectRootElement is null)
			{
				continue;
			}

			var name = projectRootElement.FullPath.GetProjectName();
			var dependencies = projectRootElement.Items.Where(x => x.ItemType is "PackageReference" or "ProjectReference").Select(x => x.Include.GetProjectName()).ToList();
			//ToDo: version

			modules.TryAdd(name, new ModuleDTO(new List<string>(), dependencies, name));
		}

		//check for required external projects
		var dlls = Directory.GetFiles(Path, "*.dll", SearchOption.AllDirectories).ToList();

		foreach (var dll in dlls)
		{
			try
			{
				var assembly = Assembly.LoadFile(dll);
				var name = assembly.GetName().Name!;
				var dependencies = assembly.GetReferencedAssemblies().Where(x => x.Name is not null).Select(x => x.Name!).ToList();
				//ToDo: version

				modules.TryAdd(name, new ModuleDTO(new List<string>(), dependencies, name));
			}
			catch
			{
				// ignored
			}
		}


		//add DataCoreDTO to DCP
		dataCoreProvider.GetOrCreateDataCore(new DataCoreDTO(modules.Values.ToList(), Guid.NewGuid())).Activate();
	}
}

internal static class StringExtensions
{
	public static string GetProjectName(this string include)
	{
		return include.Split("\\").Last().Remove(".csproj");
	}

	//public static string GetClassName(this string include)
	//{
	//	var name = include.Split("\\").Last();

	//	return name.Remove(".cs");
	//}

	//public static string GetDllName(this string include)
	//{
	//	var name = include.Split("\\").Last();

	//	return name.Remove(".dll");
	//}

	//public static string GetUsingDirective(this string include)
	//{
	//	var usingDirective = include.Split(" ").Last().Remove(";").Trim();

	//	return usingDirective;
	//}

	private static string Remove(this string str, string toRemove)
	{
		var index = str.IndexOf(toRemove, StringComparison.Ordinal);
		str = index < 0
			? str
			: str.Remove(index, toRemove.Length);
		return str;
	}
}