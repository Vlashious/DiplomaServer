using System.Collections.Concurrent;

namespace DiplomaServer.Hubs.World;

public sealed class WorldInfo
{
    public readonly ConcurrentDictionary<string, Guid> Players = new();
    public readonly ConcurrentBag<Guid> Whales = new();
}