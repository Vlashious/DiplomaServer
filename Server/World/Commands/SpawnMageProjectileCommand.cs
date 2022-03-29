using System.Numerics;
using DiplomaServer.World.Components;
using Leopotam.EcsLite;

namespace DiplomaServer.World.Commands;

public sealed class SpawnMageProjectileCommand : ICommand
{
    private readonly EcsWorld _world;
    private readonly Vector3 _spawnPos;
    private readonly int _targetId;
    private readonly float _speed;
    private readonly int _damage;

    public SpawnMageProjectileCommand(EcsWorld world, Vector3 spawnPos, int targetId, float speed, int damage)
    {
        _world = world;
        _spawnPos = spawnPos;
        _targetId = targetId;
        _speed = speed;
        _damage = damage;
    }

    public Task Execute()
    {
        int projectile = _world.NewEntity();
        _world.GetPool<Projectile>().Add(projectile) = new Projectile(_targetId, _speed, _damage);
        _world.GetPool<Position>().Add(projectile) = new Position(_spawnPos);
        return Task.CompletedTask;
    }
}