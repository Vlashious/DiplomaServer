namespace DiplomaServer.World.Components;

public struct DelayedDamageEvent
{
    public int Damage;
    public float Duration;
    public int Entity;

    public DelayedDamageEvent(int damage, int entity, float duration)
    {
        Damage = damage;
        Entity = entity;
        Duration = duration;
    }
}