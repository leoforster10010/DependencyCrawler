using System.Collections;

namespace DependencyCrawler.DataCore;

public class ReadOnlyDictionaryWrapper<TKey, TValue, TWrappedValue>(IReadOnlyDictionary<TKey, TWrappedValue> dictionary) : IReadOnlyDictionary<TKey, TValue>
	where TWrappedValue : TValue
{
	public TValue this[TKey key] => dictionary[key];

	public IEnumerable<TKey> Keys => dictionary.Keys;

	public IEnumerable<TValue> Values => dictionary.Values.Cast<TValue>();

	public int Count => dictionary.Count;

	public bool ContainsKey(TKey key)
	{
		return dictionary.ContainsKey(key);
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		return dictionary.Select(kvp => new KeyValuePair<TKey, TValue>(kvp.Key, kvp.Value)).GetEnumerator();
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		if (dictionary.TryGetValue(key, out var wrappedValue))
		{
			value = wrappedValue;
			return true;
		}

		value = default;
		return false;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}