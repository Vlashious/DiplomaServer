namespace DiplomaServer.World.Components;

public struct DamageEvent
{
    public int Entity;
    public int Damage;
    public DamageEvent(int entity, int damage)
    {
        Entity = entity;
        Damage = damage;
    }
}