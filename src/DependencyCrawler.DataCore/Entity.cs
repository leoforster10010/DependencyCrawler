namespace DependencyCrawler.DataCore;

internal partial class DataCoreProvider
{
	private partial class DataCore
	{
		private abstract class Entity(DataCore dataCore) : IEntity
		{
			protected readonly DataCore _dataCore = dataCore;
			public IDataCore DataCore => _dataCore;
			public IDataCoreProvider DataCoreProvider => _dataCore._dataCoreProvider;

			public abstract IReadOnlyList<IEntity> IngoingReferences { get; }
			public abstract IReadOnlyList<IEntity> OutgoingReferences { get; }

			public abstract void Delete();
		}
	}
}