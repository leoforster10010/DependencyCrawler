using DependencyCrawler.BlazorClient.Backend;

namespace DependencyCrawler.BlazorClient.Contracts;

internal interface IApplicationStateHandler
{
    ApplicationState ApplicationState { get; }
    void Load();
    event Action? OnStateChanged;
}