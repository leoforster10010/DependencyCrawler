using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Framework.Extensions;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.Implementations.Repositories;

internal class EvaluationRepository : IEvaluationRepository
{
    private readonly ILogger<EvaluationRepository> _logger;
    private readonly IReadOnlyProjectProvider _readOnlyProjectProvider;

    public EvaluationRepository(IReadOnlyProjectProvider readOnlyProjectProvider, ILogger<EvaluationRepository> logger)
    {
        _readOnlyProjectProvider = readOnlyProjectProvider;
        _logger = logger;
    }

    public IDictionary<Guid, IReadOnlyReference> GetUnusedDependencies()
    {
        var internalProjects = _readOnlyProjectProvider.InternalProjectsReadOnly.Values;
        var unusedReferences = new Dictionary<Guid, IReadOnlyReference>();

        foreach (var internalProject in internalProjects)
        {
            foreach (var dependency in internalProject.DependenciesReadOnly.Values)
            {
                if (internalProject.UsingDirectivesReadOnly.Values.All(x =>
                        x.ReferencedNamespaceReadOnly.ParentProjectReadOnly != dependency.UsingReadOnly))
                {
                    unusedReferences.TryAdd(dependency.Id, dependency);
                }
            }
        }

        return unusedReferences;
    }

    public IDictionary<Guid, IReadOnlyReference> GetAlreadyReferenced(
        IDictionary<Guid, IReadOnlyReference>? excludedReferences = null)
    {
        var layerGrouping = _readOnlyProjectProvider.InternalProjectsReadOnly.Values
            .GroupBy(x => x.DependencyLayerInternalReadOnly).OrderBy(x => x.Key);
        var unusedReferences = excludedReferences ?? new Dictionary<Guid, IReadOnlyReference>();
        _logger.LogInformation("Searching for already referenced Dependencies...");

        foreach (var layer in layerGrouping)
        {
            _logger.LogInformation($"Current layer: {layer.Key}");

            foreach (var readOnlyInternalProject in layer)
            {
                GetAlreadyReferencedForProject(readOnlyInternalProject, unusedReferences);
            }
        }

        return unusedReferences;
    }

    public IDictionary<Guid, IReadOnlyReference> GetUnresolvedDependencies(
        IDictionary<Guid, IReadOnlyReference>? excludedReferences = null)
    {
        var unusedReferences = excludedReferences ?? new Dictionary<Guid, IReadOnlyReference>();
        var layerGrouping = _readOnlyProjectProvider.InternalProjectsReadOnly.Values
            .GroupBy(x => x.DependencyLayerInternalReadOnly).OrderBy(x => x.Key);

        foreach (var layer in layerGrouping)
        {
            foreach (var internalProject in layer)
            {
                //var unresolvedUsingDirecives = internalProject.UsingDirectivesReadOnly.Values.DistinctBy(x => x.ReferencedNamespaceReadOnly.ParentProjectReadOnly)
                //    .Where(x => x.StateReadOnly is TypeUsingDirectiveState.Linked &&
                //                !internalProject.DependsOnReadOnly(x.ReferencedNamespaceReadOnly.ParentProjectReadOnly,
                //                    unusedReferences)).Select(x => new UnresolvedReference
                //    {
                //        Using = x.ParentProjectReadOnly,
                //        UsedBy = x.ReferencedNamespaceReadOnly.ParentProjectReadOnly,
                //    });
            }
        }

        return unusedReferences;
    }

    private void GetAlreadyReferencedForProject(IReadOnlyProject readOnlyInternalProject,
        IDictionary<Guid, IReadOnlyReference> unusedReferences)
    {
        _logger.LogInformation($"Evaluating {readOnlyInternalProject.NameReadOnly}...");

        var projectDependencies = readOnlyInternalProject.DependenciesReadOnly.Values
            .Where(x => !unusedReferences.ContainsKey(x.Id)).ToList();

        foreach (var dependency in projectDependencies)
        {
            var otherDependencies =
                projectDependencies.Where(x => x != dependency);

            if (otherDependencies.Any(
                    x => x.UsingReadOnly.DependsOnReadOnly(dependency.UsingReadOnly, unusedReferences)))
            {
                unusedReferences.TryAdd(dependency.Id, dependency);
            }
        }
    }
}