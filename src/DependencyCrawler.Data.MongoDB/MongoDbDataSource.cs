using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ValueAccess;
using MongoDB.Driver;

namespace DependencyCrawler.Data.MongoDB;

internal class MongoDbDataSource(IDataCoreProvider dataCoreProvider, IMongoDbClientProvider mongoDbClientProvider) : IDataSource
{
	public Guid Id { get; } = Guid.NewGuid();

	public async Task Save()
	{
		var activeCoreDto = dataCoreProvider.ActiveCore.ToDTO();
		await mongoDbClientProvider.DataCoreDtocCollection.ReplaceOneAsync(Builders<DataCoreDTO>.Filter.Eq(dto => dto.Id, activeCoreDto.Id), activeCoreDto, new ReplaceOptions { IsUpsert = true });
	}

	public async Task Load()
	{
		var dataCoreDtos = (await mongoDbClientProvider.DataCoreDtocCollection.FindAsync(_ => true)).ToList();
		foreach (var dataCoreDto in dataCoreDtos)
		{
			dataCoreProvider.GetOrCreateDataCore(dataCoreDto);
		}
	}
}