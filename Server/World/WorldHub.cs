using Microsoft.AspNetCore.SignalR;

namespace DiplomaServer.World;

public sealed class WorldHub : Hub
{
    private readonly MainWorld _world;

    public WorldHub(MainWorld world)
    {
        _world = world;
    }

    public void Start()
    {
        Task.Run(Update);
    }

    private async void Update()
    {
        while (true)
        {
            await Task.Delay(60);
            _world.Update();
            await UpdatePlayerPosition();
        }
    }

    private async Task UpdatePlayerPosition()
    {
        var filter = _world.World.Filter<Player>().Inc<Position>().End();

        foreach (int player in filter)
        {
            var position = _world.World.GetPool<Position>().Get(player);
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            writer.Write(player);
            writer.Write(position.X);
            writer.Write(position.Y);
            writer.Write(position.Z);
            await Clients.All.SendAsync("UpdatePlayerPosition", stream.ToArray());
        }
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
            await stream.FlushAsync();
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
        UpdateThisPlayerPosition(data);
        await Task.CompletedTask;
    }

    private void UpdateThisPlayerPosition(byte[] data)
    {
        using var stream = new MemoryStream(data);
        using var reader = new BinaryReader(stream);
        var id = reader.ReadInt32();
        var x = reader.ReadSingle();
        var y = reader.ReadSingle();
        var z = reader.ReadSingle();

        var filter = _world.World.Filter<Player>().Inc<Position>().End();

        foreach (int entity in filter)
        {
            var player = _world.World.GetPool<Player>().Get(entity);

            if (player.Id == id)
            {
                ref var pos = ref _world.World.GetPool<Position>().Get(entity);
                pos.X = x;
                pos.Y = y;
                pos.Z = z;
            }
        }
    }
}