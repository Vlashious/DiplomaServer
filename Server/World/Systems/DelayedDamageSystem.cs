﻿using DiplomaServer.World.Components;
using Leopotam.EcsLite;

namespace DiplomaServer.World.Systems;

public sealed class DelayedDamageSystem : IEcsRunSystem
{
    public void Run(EcsSystems systems)
    {
        var world = systems.GetWorld();

        foreach (int entity in world.Filter<DelayedDamageEvent>().End())
        {
            ref var damageEvent = ref world.GetPool<DelayedDamageEvent>().Get(entity);

            if (DateTime.Now >= damageEvent.FireTime)
            {
                var dealDamageEvent = world.NewEntity();
                world.GetPool<DamageEvent>().Add(dealDamageEvent) = new DamageEvent(damageEvent.Entity, damageEvent.Damage);
                world.DelEntity(entity);
            }
        }
    }
}