using System.Collections.Concurrent;

namespace DependencyCrawler.DataCore;

internal partial class DataCoreProvider : IDataCoreProvider
{
	private readonly IDictionary<Guid, IDataCore> _dataCores = new ConcurrentDictionary<Guid, IDataCore>();
	private DataCore _activeCore;

	public DataCoreProvider()
	{
		_activeCore = new DataCore(this);
	}

	public IDataCore ActiveCore => _activeCore;
	public IReadOnlyDictionary<Guid, IDataCore> DataCores => _dataCores.AsReadOnly();

	public IDataCore CreateDataCore()
	{
		return new DataCore(this);
	}
}