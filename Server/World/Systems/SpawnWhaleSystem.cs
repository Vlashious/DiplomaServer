using System.Numerics;
using DiplomaServer.World.Components;
using Leopotam.EcsLite;

namespace DiplomaServer.World.Systems;

public sealed class SpawnWhaleSystem : IEcsInitSystem
{
    public void Init(EcsSystems systems)
    {
        for (int i = 0; i < 4; i++)
        {
            var world = systems.GetWorld();
            var whale = world.NewEntity();
            world.GetPool<Whale>().Add(whale) = new Whale(whale);
            world.GetPool<Position>().Add(whale) = new Position(new Vector3(i * 4, 1, 4));
            world.GetPool<Health>().Add(whale) = new Health(300);
        }
    }
}