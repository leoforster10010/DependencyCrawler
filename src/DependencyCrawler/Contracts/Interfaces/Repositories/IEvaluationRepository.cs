using DependencyCrawler.Contracts.Interfaces.Model;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

public interface IEvaluationRepository
{
    IDictionary<Guid, IReadOnlyReference> GetUnusedDependencies();

    IDictionary<Guid, IReadOnlyReference> GetAlreadyReferenced(
        IDictionary<Guid, IReadOnlyReference>? excludedReferences = null);

    IDictionary<Guid, IReadOnlyReference> GetUnresolvedDependencies(
        IDictionary<Guid, IReadOnlyReference>? excludedReferences = null);
}