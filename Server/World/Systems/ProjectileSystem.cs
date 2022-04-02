using System.Numerics;
using DiplomaServer.World.Components;
using Leopotam.EcsLite;

namespace DiplomaServer.World.Systems;

public sealed class ProjectileSystem : IEcsRunSystem
{
    public void Run(EcsSystems systems)
    {
        var world = systems.GetWorld();

        foreach (int entity in world.Filter<Projectile>().Inc<Position>().End())
        {
            var projectile = world.GetPool<Projectile>().Get(entity);
            ref var projectilePos = ref world.GetPool<Position>().Get(entity);

            if (world.GetPool<Position>().Has(projectile.TargetId))
            {
                var targetPosition = world.GetPool<Position>().Get(projectile.TargetId);
                var dir = targetPosition.Value - projectilePos.Value;
                dir = Vector3.Normalize(dir);
                dir *= projectile.Speed;
                dir *= MainWorld.Delta;
                projectilePos.Value += dir;
                var distance = (projectilePos.Value - targetPosition.Value).LengthSquared();

                if (distance < 1f)
                {
                    var damageEvent = world.NewEntity();
                    world.GetPool<DamageEvent>().Add(damageEvent) = new DamageEvent(projectile.TargetId, 10);
                    world.DelEntity(entity);
                }
            }
            else
            {
                world.DelEntity(entity);
            }
        }
    }
}