using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace DiplomaServer.Hubs;

public interface IWorldHub
{
    Task RetreiveAllPlayers(Guid[] allPlayers);
    Task SendTransformUpdate(Guid id, byte[] data);
    Task OnPlayerConnected(Guid userId);
    Task OnPlayerDisconnected(Guid userId);
}

public sealed class WorldHub : Hub<IWorldHub>
{
    private static readonly ConcurrentDictionary<string, Guid> _players = new();
    private static readonly ConcurrentDictionary<string, Guid> _creatures = new();

    public async Task SendTransformUpdate(Guid id, byte[] data)
    {
        await Clients.Others.SendTransformUpdate(id, data);
    }

    public async Task OnPlayerConnected(Guid userId)
    {
        _players.TryAdd(Context.ConnectionId, userId);
        await Clients.Others.OnPlayerConnected(userId);
    }

    public async Task OnPlayerDisconnected(Guid userId)
    {
        await Clients.Others.OnPlayerDisconnected(userId);
        _players.TryRemove(Context.ConnectionId, out var removed);
    }

    public async Task RetreiveAllPlayers()
    {
        var allPlayers = _players.Values.ToArray();
        await Clients.Caller.RetreiveAllPlayers(allPlayers);
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"{Context.ConnectionId} has connected.");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"{Context.ConnectionId} has disconnected.");
        await OnPlayerDisconnected(_players[Context.ConnectionId]);
    }
}