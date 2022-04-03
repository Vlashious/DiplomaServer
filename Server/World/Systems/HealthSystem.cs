using DiplomaServer.World.Components;
using Leopotam.EcsLite;

namespace DiplomaServer.World.Systems;

public sealed class HealthSystem : IEcsRunSystem
{
    public void Run(EcsSystems systems)
    {
        var world = systems.GetWorld();

        foreach (int entity in world.Filter<Health>().End())
        {
            var health = world.GetPool<Health>().Get(entity);

            if (health.Value <= 0)
            {
                world.DelEntity(entity);
            }
        }
    }
}