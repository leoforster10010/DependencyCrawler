using System.Collections;

namespace DependencyCrawler.DataCore;

public class ReadOnlyListWrapper<T, TBase>(IEnumerable<T> collection) : IReadOnlyList<TBase>
	where T : TBase
{
	public TBase this[int index] => collection.ElementAt(index);

	public int Count => collection.Count();

	public IEnumerator<TBase> GetEnumerator()
	{
		return collection.Cast<TBase>().GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}