namespace DependencyCrawler.Framework.Extensions;

public static class ListExtensions
{
    public static void RemoveAfterValue<T>(this List<T> list, T value)
    {
        var index = list.IndexOf(value) + 1;
        if (index >= 0)
        {
            list.RemoveRange(index, list.Count - index);
        }
    }
}