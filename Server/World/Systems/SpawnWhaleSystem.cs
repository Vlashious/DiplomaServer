using System.Numerics;
using DiplomaServer.World.Components;
using Leopotam.EcsLite;

namespace DiplomaServer.World.Systems;

public sealed class SpawnWhaleSystem : IEcsInitSystem
{
    public void Init(EcsSystems systems)
    {
        var world = systems.GetWorld();
        var whale = world.NewEntity();
        world.GetPool<Whale>().Add(whale) = new Whale(whale);
        world.GetPool<Position>().Add(whale) = new Position(new Vector3(10, 1, 4));
        world.GetPool<Health>().Add(whale) = new Health(200);
    }
}