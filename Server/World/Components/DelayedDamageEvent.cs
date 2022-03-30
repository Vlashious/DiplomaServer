namespace DiplomaServer.World.Components;

public struct DelayedDamageEvent
{
    public int Damage;
    public float Delay;
    public int Entity;

    public DelayedDamageEvent(int damage, float delay, int entity)
    {
        Damage = damage;
        Delay = delay;
        Entity = entity;
    }
}