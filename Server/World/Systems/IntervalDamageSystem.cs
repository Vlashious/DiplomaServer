using DiplomaServer.World.Components;
using Leopotam.EcsLite;

namespace DiplomaServer.World.Systems;

public class IntervalDamageSystem : IEcsRunSystem
{
    public void Run(EcsSystems systems)
    {
        var world = systems.GetWorld();

        foreach (int entity in world.Filter<IntervalDamageEvent>().End())
        {
            ref var curse = ref world.GetPool<IntervalDamageEvent>().Get(entity);
            curse.ElapsedTime += MainWorld.Delta;

            if (curse.ElapsedTime >= curse.DamageInterval)
            {
                var damageEvent = world.NewEntity();
                world.GetPool<DamageEvent>().Add(damageEvent) = new DamageEvent(curse.TargetEntity, curse.DamageInInterval);
                curse.ElapsedTime -= curse.DamageInterval;
            }

            curse.Duration -= MainWorld.Delta;

            if (curse.Duration <= 0)
            {
                world.DelEntity(entity);
            }
        }
    }
}