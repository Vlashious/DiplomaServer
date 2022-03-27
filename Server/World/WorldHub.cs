using DiplomaServer.World.Commands;
using DiplomaServer.World.Components;
using Microsoft.AspNetCore.SignalR;

namespace DiplomaServer.World;

public sealed class WorldHub : Hub
{
    private readonly MainWorld _world;

    public WorldHub(MainWorld world)
    {
        _world = world;
    }

    public override async Task OnConnectedAsync()
    {
        var connectedPlayer = _world.World.NewEntity();
        var position = new Position(0, 0, 0);
        _world.World.GetPool<Player>().Add(connectedPlayer) = new Player(connectedPlayer, Context.ConnectionId);
        _world.World.GetPool<Position>().Add(connectedPlayer) = position;

        await using var stream = new MemoryStream();
        await using var writer = new BinaryWriter(stream);
        writer.Write(connectedPlayer);
        writer.Write(position.X);
        writer.Write(position.Y);
        writer.Write(position.Z);
        await Clients.Caller.SendAsync("SpawnPlayer", stream.ToArray());
        await Clients.Others.SendAsync("SpawnNetworkPlayer", stream.ToArray());

        var filter = _world.World.Filter<Player>().Inc<Position>().End();

        foreach (int entity in filter)
        {
            if (entity == connectedPlayer)
            {
                continue;
            }

            var playerId = _world.World.GetPool<Player>().Get(entity);
            var playerPosition = _world.World.GetPool<Position>().Get(entity);
            stream.SetLength(0);
            writer.Write(playerId.Id);
            writer.Write(playerPosition.X);
            writer.Write(playerPosition.Y);
            writer.Write(playerPosition.Z);
            await Clients.Caller.SendAsync("SpawnNetworkPlayer", stream.ToArray());
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var filter = _world.World.Filter<Player>().Inc<Position>().End();

        foreach (int player in filter)
        {
            var id = _world.World.GetPool<Player>().Get(player);

            if (id.HudConnectionId == Context.ConnectionId)
            {
                await using var stream = new MemoryStream();
                await using var writer = new BinaryWriter(stream);
                writer.Write(player);

                await Clients.Others.SendAsync("DestroyPlayer", stream.ToArray());
                _world.World.DelEntity(player);
            }
        }
    }

    public async Task SendPlayerData(byte[] data)
    {
        _world.Commands.Enqueue(new UpdatePlayerPositionCommand(data, _world.World));
        await Clients.Others.SendAsync("UpdatePlayerPosition", data);
    }
}