using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Models.LinkedTypes;

namespace DependencyCrawler.Implementations.Models;

public class DependencyPathInfo : Entity
{
    public required IReadOnlyList<IReadOnlyList<IReadOnlyProject>> PossiblePaths { get; init; }
    public required IReadOnlyProject Project { get; init; }
    public required IReadOnlyProject Dependency { get; init; }
    public int DistanceShortestPath => PossiblePaths.MinBy(x => x.Count)?.Count - 1 ?? 0;
    public int DistanceLongestPath => PossiblePaths.MaxBy(x => x.Count)?.Count - 1 ?? 0;
    public double AveragePathDistance => PossiblePaths.Any() ? PossiblePaths.Select(x => x.Count).Average() : 0;
}