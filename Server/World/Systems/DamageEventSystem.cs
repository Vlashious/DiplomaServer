using DiplomaServer.World.Components;
using Leopotam.EcsLite;
using Microsoft.AspNetCore.SignalR;

namespace DiplomaServer.World.Systems;

public sealed class DamageEventSystem : IEcsRunSystem
{
    private readonly MainWorld _mainWorld;

    public DamageEventSystem(MainWorld mainWorld)
    {
        _mainWorld = mainWorld;
    }

    public void Run(EcsSystems systems)
    {
        var world = systems.GetWorld();

        foreach (int entity in world.Filter<DamageEvent>().End())
        {
            var damageEvent = world.GetPool<DamageEvent>().Get(entity);
            var healthPool = world.GetPool<Health>();

            if (healthPool.Has(damageEvent.Entity))
            {
                ref var health = ref healthPool.Get(damageEvent.Entity);
                health.Value -= CalculateDamageForEntity(world, damageEvent.Entity, damageEvent.Damage);
                using var ms = new MemoryStream();
                using var writer = new BinaryWriter(ms);
                writer.Write(damageEvent.Entity);
                writer.Write(health.Value);
                var bytes = ms.ToArray();

                _mainWorld.NeedSendData.Invoke(async hub => { await hub.Clients.All.SendAsync("UpdateHealth", bytes); });
            }
            else
            {
                world.DelEntity(entity);
            }

            world.DelEntity(entity);
        }
    }

    private int CalculateDamageForEntity(EcsWorld world, int entity, int baseDamage)
    {
        float coefficient = 1;
        var cursePool = world.GetPool<IntervalDamageEvent>();

        foreach (int cursedEntity in world.Filter<IntervalDamageEvent>().End())
        {
            var curse = cursePool.Get(cursedEntity);

            if (curse.TargetEntity == entity)
            {
                coefficient += curse.DamageIncreasePercent;
                break;
            }
        }

        return (int) Math.Round(baseDamage * coefficient);
    }
}