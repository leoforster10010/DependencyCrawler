using DependencyCrawler.Implementations.Models.UnlinkedTypes;
using Microsoft.Build.Construction;

namespace DependencyCrawler.Framework.Extensions;

internal static class ProjectRootElementExtensions
{
	public static IEnumerable<PackageReferenceInfo> GetPackageReferences(this ProjectRootElement projectRootElement)
	{
		var packageReferences = projectRootElement.Items.Where(x => x.ItemType == "PackageReference").Select(x =>
			new PackageReferenceInfo
			{
				Using = x.Include,
				UsedBy = projectRootElement.FullPath.GetProjectName()
				//Version = x. ToDo Get Version from Childs
			});

		return packageReferences;
	}

	public static IEnumerable<ProjectReferenceInfo> GetProjectReferences(this ProjectRootElement projectRootElement)
	{
		var projectReferences = projectRootElement.Items.Where(x => x.ItemType == "ProjectReference").Select(x =>
			new ProjectReferenceInfo
			{
				Using = x.Include.GetProjectName(),
				UsedBy = projectRootElement.FullPath.GetProjectName()
			});

		return projectReferences;
	}
}