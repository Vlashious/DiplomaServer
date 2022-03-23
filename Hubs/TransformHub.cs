using Microsoft.AspNetCore.SignalR;

namespace DiplomaServer.Hubs;

public interface IWorldHub
{
    Task SendTransformUpdate(string userId, byte[] data);
    Task OnPlayerConnected(string userId);
}

public sealed class TransformHub : Hub<IWorldHub>
{
    public async Task SendTransformUpdate(string userId, byte[] data)
    {
        await Clients.Others.SendTransformUpdate(userId, data);
    }

    public async Task OnPlayerConnected(string userId)
    {
        await Clients.Others.OnPlayerConnected(userId);
    }

    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"{Context.ConnectionId} has connected.");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"{Context.ConnectionId} has disconnected.");
        return base.OnDisconnectedAsync(exception);
    }
}