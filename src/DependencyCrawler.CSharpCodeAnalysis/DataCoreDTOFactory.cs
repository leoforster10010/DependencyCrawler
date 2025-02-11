using System.Reflection;
using DependencyCrawler.DataCore.ValueAccess;
using Microsoft.Build.Construction;
using Microsoft.Extensions.Configuration;

namespace DependencyCrawler.CSharpCodeAnalysis;

public class DataCoreDTOFactory(IConfiguration configuration) : IDataCoreDTOFactory
{
	public DataCoreDTO CreateDataCoreDTO()
	{
		//find all project files
		var path = configuration.GetSection(nameof(CSharpCodeAnalysisSettings)).Get<CSharpCodeAnalysisSettings>()!.RootDirectory;
		var csProjSearchPattern = configuration.GetSection(nameof(CSharpCodeAnalysisSettings)).Get<CSharpCodeAnalysisSettings>()!.CsProjSearchPattern;
		var dllSearchPattern = configuration.GetSection(nameof(CSharpCodeAnalysisSettings)).Get<CSharpCodeAnalysisSettings>()!.DllSearchPattern;
		var packageReferenceIdentifier = configuration.GetSection(nameof(CSharpCodeAnalysisSettings)).Get<CSharpCodeAnalysisSettings>()!.PackageReferenceIdentifier;
		var projectReferenceIdentifier = configuration.GetSection(nameof(CSharpCodeAnalysisSettings)).Get<CSharpCodeAnalysisSettings>()!.ProjectReferenceIdentifier;

		var projectFiles = Directory.GetFiles(path, csProjSearchPattern, SearchOption.AllDirectories);

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
			var dependencies = projectRootElement.Items.Where(x => x.ItemType == packageReferenceIdentifier || x.ItemType == projectReferenceIdentifier).Select(x => x.Include.GetProjectName()).ToList();
			//ToDo: version

			modules.TryAdd(name, new ModuleDTO(new List<string>(), dependencies, name));
		}

		//check for required external projects
		var dlls = Directory.GetFiles(path, dllSearchPattern, SearchOption.AllDirectories).Where(dll => !modules.ContainsKey(dll.GetDllName())).ToList();

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

		return new DataCoreDTO(modules.Values.ToList(), Guid.NewGuid());
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