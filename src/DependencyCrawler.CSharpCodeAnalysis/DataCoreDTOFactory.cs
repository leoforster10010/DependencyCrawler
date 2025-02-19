using System.Collections.Concurrent;
using System.Reflection;
using DependencyCrawler.DataCore.ValueAccess;
using Microsoft.Build.Construction;
using Microsoft.Extensions.Configuration;

namespace DependencyCrawler.CSharpCodeAnalysis;

public class DataCoreDTOFactory(IConfiguration configuration) : IDataCoreDTOFactory
{
	public DataCoreDTO CreateDataCoreDTO()
	{
		var settings = configuration.GetSection(nameof(CSharpCodeAnalysisSettings)).Get<CSharpCodeAnalysisSettings>()!;
		var path = settings.RootDirectory;
		var csProjSearchPattern = settings.CsProjSearchPattern;
		var dllSearchPattern = settings.DllSearchPattern;
		var packageReferenceIdentifier = settings.PackageReferenceIdentifier;
		var projectReferenceIdentifier = settings.ProjectReferenceIdentifier;

		//find all project files
		var projectFiles = Directory.GetFiles(path, csProjSearchPattern, SearchOption.AllDirectories);

		var modules = new ConcurrentDictionary<string, ModuleDTO>();
		var requiredDependencies = new ConcurrentDictionary<string, bool>();

		//extract info to ModuleDTO
		Parallel.ForEach(projectFiles, projectFile =>
		{
			var projectRootElement = ProjectRootElement.Open(projectFile);

			if (projectRootElement is null)
			{
				return;
			}

			var name = projectRootElement.FullPath.GetProjectName();
			var dependencies = projectRootElement.Items
				.Where(x => x.ItemType == packageReferenceIdentifier || x.ItemType == projectReferenceIdentifier)
				.Select(x => x.Include.GetProjectName())
				.ToList();

			modules.TryAdd(name, new ModuleDTO(new List<string>(), dependencies, name));
			Parallel.ForEach(dependencies, dependency => { requiredDependencies.TryAdd(dependency, true); });
		});

		//check for required external projects
		var dlls = Directory.GetFiles(path, dllSearchPattern, SearchOption.AllDirectories)
			.Where(dll => !modules.ContainsKey(dll.GetDllName()))
			.GroupBy(x => x.GetDllName())
			.ToDictionary(x => x.Key.GetDllName(), x => x.First());

		while (!requiredDependencies.IsEmpty)
		{
			var dependency = requiredDependencies.Keys.First();
			requiredDependencies.TryRemove(dependency, out _);

			if (modules.ContainsKey(dependency))
			{
				continue;
			}

			if (!dlls.TryGetValue(dependency, out dependency))
			{
				continue;
			}

			try
			{
				if (!IsManagedAssembly(dependency))
				{
					continue;
				}

				var assembly = Assembly.LoadFile(dependency);
				var name = assembly.GetName().Name!;
				var dependencies = assembly.GetReferencedAssemblies()
					.Where(x => x.Name is not null)
					.Select(x => x.Name!)
					.ToList();

				foreach (var dep in dependencies)
				{
					requiredDependencies.TryAdd(dep, true);
				}

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
			// no valid .NET-Assembly
			return false;
		}
		catch (FileNotFoundException)
		{
			return false;
		}
	}
}