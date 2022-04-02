using System.Numerics;
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
        _world.NeedSendData += OnNeedSendData;
    }

    private async Task OnNeedSendData(Func<Hub, Task> arg)
    {
        await arg.Invoke(this);
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine("Player connected.");
        _world.IsTicking = false;
        var connectedPlayer = _world.World.NewEntity();
        var position = new Position(new Vector3(0, 0, 0));
        var health = 500;
        _world.World.GetPool<Player>().Add(connectedPlayer) = new Player(connectedPlayer, Context.ConnectionId);
        _world.World.GetPool<Position>().Add(connectedPlayer) = position;
        _world.World.GetPool<Health>().Add(connectedPlayer) = new Health(health);
        _world.IsTicking = true;

        await using var stream = new MemoryStream();
        await using var writer = new BinaryWriter(stream);
        writer.Write(connectedPlayer);
        writer.Write(position.Value.X);
        writer.Write(position.Value.Y);
        writer.Write(position.Value.Z);
        writer.Write(health);
        await Clients.Caller.SendAsync("SpawnPlayer", stream.ToArray());
        await Clients.Others.SendAsync("SpawnNetworkPlayer", stream.ToArray());

        var filter = _world.World.Filter<Player>().Inc<Position>().Inc<Health>().End();

        foreach (int entity in filter)
        {
            if (entity == connectedPlayer)
            {
                continue;
            }

            var playerId = _world.World.GetPool<Player>().Get(entity);
            var playerPosition = _world.World.GetPool<Position>().Get(entity);
            var playerHealth = _world.World.GetPool<Health>().Get(entity);
            stream.SetLength(0);
            writer.Write(playerId.Id);
            writer.Write(playerPosition.Value.X);
            writer.Write(playerPosition.Value.Y);
            writer.Write(playerPosition.Value.Z);
            writer.Write(playerHealth.Value);
            await Clients.Caller.SendAsync("SpawnNetworkPlayer", stream.ToArray());
        }

        filter = _world.World.Filter<Whale>().Inc<Position>().Inc<Health>().End();

        foreach (int entity in filter)
        {
            var whale = _world.World.GetPool<Whale>().Get(entity);
            var pos = _world.World.GetPool<Position>().Get(entity);
            var whaleHealth = _world.World.GetPool<Health>().Get(entity);
            stream.SetLength(0);
            writer.Write(whale.Id);
            writer.Write(pos.Value.X);
            writer.Write(pos.Value.Y);
            writer.Write(pos.Value.Z);
            writer.Write(whaleHealth.Value);
            await Clients.Caller.SendAsync("SpawnWhale", stream.ToArray());
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

    public async Task SpawnMageProjectile(byte[] data)
    {
        await using var readMs = new MemoryStream(data);
        using var rd = new BinaryReader(readMs);
        var targetId = rd.ReadInt32();
        var startX = rd.ReadSingle();
        var startY = rd.ReadSingle();
        var startZ = rd.ReadSingle();

        int damage = 10;
        float speed = 20;
        await using var writeMs = new MemoryStream();
        await using var wr = new BinaryWriter(writeMs);
        wr.Write(targetId);
        wr.Write(startX);
        wr.Write(startY);
        wr.Write(startZ);
        wr.Write(speed);
        _world.Commands.Enqueue(new SpawnMageProjectileCommand(_world.World, new Vector3(startX, startY, startZ), targetId, speed, damage));
        await Clients.All.SendAsync("SpawnMageProjectile", writeMs.ToArray());
    }

    public async Task SpawnMageBomb(byte[] data)
    {
        await using var readMs = new MemoryStream(data);
        using var rd = new BinaryReader(readMs);
        var targetId = rd.ReadInt32();
        float duration = 5;
        _world.Commands.Enqueue(new SpawnMageBombCommand(_world.World, duration, targetId));

        await using var ms = new MemoryStream();
        await using var wr = new BinaryWriter(ms);
        wr.Write(targetId);
        wr.Write(duration);
        await Clients.All.SendAsync("SpawnMageBomb", ms.ToArray());
    }
}