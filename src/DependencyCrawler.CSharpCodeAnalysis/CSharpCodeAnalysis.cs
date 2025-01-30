using System.Reflection;
using DependencyCrawler.DataCore;
using DependencyCrawler.DataCore.ValueAccess;
using Microsoft.Build.Construction;

namespace DependencyCrawler.CSharpCodeAnalysis;

internal class CSharpCodeAnalysis(IDataCoreProvider dataCoreProvider) : ICodeAnalysis
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
		var dlls = Directory.GetFiles(Path, "*.dll", SearchOption.AllDirectories).Where(dll => !modules.ContainsKey(dll.GetDllName())).ToList();

		foreach (var dll in dlls.Where(dll => !modules.ContainsKey(dll.GetDllName())))
		{
			try
			{
				if (!IsManagedAssembly(dll))
				{
					continue;
				}

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

	private static bool IsManagedAssembly(string fileName)
	{
		try
		{
			AssemblyName.GetAssemblyName(fileName);
			return true;
		}
		catch (BadImageFormatException)
		{
			// Keine gültige .NET-Assembly
			return false;
		}
		catch (FileNotFoundException)
		{
			return false;
		}
	}
}