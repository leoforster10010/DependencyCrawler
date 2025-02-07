namespace DependencyCrawler.CSharpCodeAnalysis.REST;

internal static class StringExtensions
{
	public static string GetProjectName(this string include)
	{
		return include.Split("\\").Last().Remove(".csproj");
	}

	public static string GetDllName(this string include)
	{
		return include.Split("\\").Last().Remove(".dll");
	}

	private static string Remove(this string str, string toRemove)
	{
		var index = str.IndexOf(toRemove, StringComparison.Ordinal);
		str = index < 0
			? str
			: str.Remove(index, toRemove.Length);
		return str;
	}
}