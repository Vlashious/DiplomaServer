namespace DiplomaServer.World.Components;

public struct DelayedDamageEvent
{
    public int Damage;
    public DateTime FireTime;
    public int Entity;

    public DelayedDamageEvent(int damage, int entity, DateTime fireTime)
    {
        Damage = damage;
        Entity = entity;
        FireTime = fireTime;
    }
}