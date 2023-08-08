namespace DependencyCrawler.Framework.Extensions;

internal static class StringExtensions
{
	public static string GetProjectName(this string include)
	{
		var name = include.Split("\\").Last();

		return name.Remove(".csproj");
	}

	public static string GetClassName(this string include)
	{
		var name = include.Split("\\").Last();

		return name.Remove(".cs");
	}

	public static string GetDllName(this string include)
	{
		var name = include.Split("\\").Last();

		return name.Remove(".dll");
	}

	public static string GetUsingDirective(this string include)
	{
		var usingDirective = include.Split(" ").Last().Remove(";").Trim();

		return usingDirective;
	}

	public static string Remove(this string str, string toRemove)
	{
		var index = str.IndexOf(toRemove, StringComparison.Ordinal);
		str = index < 0
			? str
			: str.Remove(index, toRemove.Length);
		return str;
	}
}