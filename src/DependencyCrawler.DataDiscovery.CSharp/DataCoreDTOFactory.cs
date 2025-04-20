using System.Reflection;
using DependencyCrawler.DataCore.ValueAccess;
using Microsoft.Build.Construction;
using Microsoft.Extensions.Configuration;

namespace DependencyCrawler.DataDiscovery.CSharp;

public class DataCoreDTOFactory(IConfiguration configuration) : IDataCoreDTOFactory
{
	public DataCoreDTO CreateDataCoreDTO(string? filePath = null)
	{
		var settings = configuration.GetSection(nameof(CSharpDataDiscoverySettings)).Get<CSharpDataDiscoverySettings>()!;
		var path = filePath ?? settings.RootDirectory;
		var csProjSearchPattern = settings.CsProjSearchPattern;
		var dllSearchPattern = settings.DllSearchPattern;
		var packageReferenceIdentifier = settings.PackageReferenceIdentifier;
		var projectReferenceIdentifier = settings.ProjectReferenceIdentifier;

		//find all project files
		var projectFiles = Directory.GetFiles(path, csProjSearchPattern, SearchOption.AllDirectories);

		var modules = new Dictionary<string, IValueModule>();
		var requiredDependencies = new HashSet<string>();

		//extract info to ModuleDTO
		foreach (var projectFile in projectFiles)
		{
			var projectRootElement = ProjectRootElement.Open(projectFile);

			if (projectRootElement?.FullPath is null)
			{
				continue;
			}

			var name = projectRootElement.FullPath.GetProjectName();
			var dependencies = projectRootElement.Items
				.Where(x => x.Include != null && (x.ItemType == packageReferenceIdentifier || x.ItemType == projectReferenceIdentifier))
				.Select(x => x.Include.GetProjectName())
				.ToList();

			modules.TryAdd(name, new ModuleDTO
			{
				Name = name,
				Type = ModuleType.Internal,
				DependencyValues = dependencies,
				ReferenceValues = new List<string>()
			});

			requiredDependencies.UnionWith(dependencies);
		}

		//check for required external projects
		var dlls = Directory.GetFiles(path, dllSearchPattern, SearchOption.AllDirectories)
			.Where(dll => !modules.ContainsKey(dll.GetDllName()))
			.GroupBy(x => x.GetDllName())
			.ToDictionary(x => x.Key.GetDllName(), x => x.First());

		while (requiredDependencies.Count is not 0)
		{
			var dependency = requiredDependencies.First();
			requiredDependencies.Remove(dependency);

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

				requiredDependencies.UnionWith(dependencies);

				modules.TryAdd(name, new ModuleDTO
				{
					Name = name,
					Type = ModuleType.External,
					DependencyValues = dependencies,
					ReferenceValues = new List<string>()
				});
			}
			catch
			{
				// ignored
			}
		}

		return new DataCoreDTO
		{
			Id = Guid.NewGuid(),
			ModuleValues = modules
		};
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