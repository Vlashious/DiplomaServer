using DiplomaServer.World.Components;
using Leopotam.EcsLite;

namespace DiplomaServer.World.Commands;

public sealed class SpawnMageBombCommand : ICommand
{
    private readonly EcsWorld _world;
    private readonly float _duration;
    private readonly int _targetId;

    public SpawnMageBombCommand(EcsWorld world, float duration, int targetId)
    {
        _world = world;
        _duration = duration;
        _targetId = targetId;
    }

    public async Task Execute()
    {
        var entity = _world.NewEntity();
        _world.GetPool<DelayedDamageEvent>().Add(entity) = new DelayedDamageEvent(50, _targetId, _duration);
        await Task.CompletedTask;
    }
}