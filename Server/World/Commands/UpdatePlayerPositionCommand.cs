using DiplomaServer.World.Components;
using Leopotam.EcsLite;

namespace DiplomaServer.World.Commands;

public sealed class UpdatePlayerPositionCommand : ICommand
{
    private readonly byte[] _data;
    private readonly EcsWorld _world;

    public UpdatePlayerPositionCommand(byte[] data, EcsWorld world)
    {
        _data = data;
        _world = world;
    }

    public Task Execute()
    {
        using var stream = new MemoryStream(_data);
        using var reader = new BinaryReader(stream);
        var id = reader.ReadInt32();
        var x = reader.ReadSingle();
        var y = reader.ReadSingle();
        var z = reader.ReadSingle();
        var rx = reader.ReadSingle();
        var ry = reader.ReadSingle();
        var rz = reader.ReadSingle();

        foreach (int playerEntity in _world.Filter<Player>().Inc<Position>().Inc<Rotation>().End())
        {
            var player = _world.GetPool<Player>().Get(playerEntity);

            if (player.Id == id)
            {
                ref var pos = ref _world.GetPool<Position>().Get(playerEntity);
                ref var rot = ref _world.GetPool<Rotation>().Get(playerEntity);
                pos.Value.X = x;
                pos.Value.Y = y;
                pos.Value.Z = z;
                rot.X = rx;
                rot.Y = ry;
                rot.Z = rz;
                break;
            }
        }

        return Task.CompletedTask;
    }
}