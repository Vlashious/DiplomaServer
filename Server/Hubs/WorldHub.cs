using DiplomaServer.Hubs.World;
using Microsoft.AspNetCore.SignalR;

namespace DiplomaServer.Hubs;

public interface IWorldHub
{
    Task SendTransformUpdate(Guid id, byte[] data);
    Task ThisPlayerConnect(Guid userId, Guid[] other);
    Task OtherPlayerConnect(Guid userId);
    Task OnPlayerDisconnected(Guid userId);
}

public sealed class WorldHub : Hub<IWorldHub>
{
    private readonly WorldInfo _worldInfo;

    public WorldHub(WorldInfo worldInfo)
    {
        _worldInfo = worldInfo;
    }

    public async Task SendTransformUpdate(Guid id, byte[] data)
    {
        await Clients.Others.SendTransformUpdate(id, data);
    }

    public async Task ThisPlayerConnect(Guid userId, Guid[] other)
    {
        _worldInfo.Players.TryAdd(Context.ConnectionId, userId);
        other = _worldInfo.Players.Values.Except(new[] {userId}).ToArray();
        await Clients.Caller.ThisPlayerConnect(userId, other);
        await Clients.Others.OtherPlayerConnect(userId);
    }

    public async Task OnPlayerDisconnected(Guid userId)
    {
        await Clients.Others.OnPlayerDisconnected(userId);
        _worldInfo.Players.TryRemove(Context.ConnectionId, out var removed);
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"{Context.ConnectionId} has connected.");
        await Task.CompletedTask;
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"{Context.ConnectionId} has disconnected.");
        await OnPlayerDisconnected(_worldInfo.Players[Context.ConnectionId]);
    }
}