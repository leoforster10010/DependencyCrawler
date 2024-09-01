using System.Collections.Concurrent;
using DependencyCrawler.DataCore.ReadOnlyAccess;
using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore;

public partial class DataCoreProvider : IDataCoreProvider
{
	private readonly IDictionary<Guid, IDataCore> _dataCores = new ConcurrentDictionary<Guid, IDataCore>();
	private DataCore _activeCore;

	public DataCoreProvider()
	{
		_activeCore = new DataCore(this);
	}

	public IDataCore ActiveCore => _activeCore;
	public IReadOnlyDataCore DataReadOnly => _activeCore;
	public IValueDataCore DataValues => _activeCore;
	public IReadOnlyDictionary<Guid, IDataCore> DataCores => _dataCores.AsReadOnly();

	public IDataCore CreateDataCore()
	{
		return new DataCore(this);
	}
}