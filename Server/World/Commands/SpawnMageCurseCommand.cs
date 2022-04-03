using System.Numerics;
using DiplomaServer.World.Components;
using Leopotam.EcsLite;
using Microsoft.AspNetCore.SignalR;

namespace DiplomaServer.World.Commands;

public sealed class SpawnMageCurseCommand : ICommand
{
    private readonly MainWorld _world;
    private readonly Vector3 _spawnPos;
    private readonly float _duration;
    private readonly int _damageInInterval;
    private readonly float _damageInterval;
    private readonly float _increasePercent;

    public SpawnMageCurseCommand(MainWorld world, Vector3 spawnPos, float duration, int damageInInterval, float damageInterval,
        float increasePercent)
    {
        _world = world;
        _spawnPos = spawnPos;
        _duration = duration;
        _damageInInterval = damageInInterval;
        _damageInterval = damageInterval;
        _increasePercent = increasePercent;
    }

    public async Task Execute()
    {
        var curseRadius = 2.5f;
        var affectedEntities = new List<int>();

        foreach (int entity in _world.World.Filter<Health>().Inc<Position>().End())
        {
            var pos = _world.World.GetPool<Position>().Get(entity);
            var distance = (pos.Value - _spawnPos).Length();

            if (distance <= curseRadius)
            {
                var curseEntity = _world.World.NewEntity();

                _world.World.GetPool<IntervalDamageEvent>().Add(curseEntity) =
                    new IntervalDamageEvent(_duration, _damageInInterval, _damageInterval, _increasePercent, entity);
                affectedEntities.Add(entity);
            }
        }

        await using var ms = new MemoryStream();
        await using var wr = new BinaryWriter(ms);
        wr.Write(_spawnPos.X);
        wr.Write(_spawnPos.Y);
        wr.Write(_spawnPos.Z);
        wr.Write(_duration);
        wr.Write(affectedEntities.Count);

        foreach (int affectedEntity in affectedEntities)
        {
            wr.Write(affectedEntity);
        }

        await _world.NeedSendData.Invoke(async hub => { await hub.Clients.All.SendAsync("SpawnMageCurse", ms.ToArray()); });

        await Task.CompletedTask;
    }
}