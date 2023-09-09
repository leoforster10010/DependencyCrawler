namespace DependencyCrawler.Framework.Extensions;

internal static class TypeExtensions
{
    public static string GetNamespace(this Type type)
    {
        return type.FullName!.Remove(type.Name).Remove("+");
    }
}