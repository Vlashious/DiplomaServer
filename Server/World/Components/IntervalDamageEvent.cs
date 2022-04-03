namespace DiplomaServer.World.Components;

public struct IntervalDamageEvent
{
    public int TargetEntity;
    public float Duration;
    public int DamageInInterval;
    public float DamageInterval;
    public float DamageIncreasePercent;
    public float ElapsedTime;

    public IntervalDamageEvent(float duration, int damageInInterval, float damageInterval, float damageIncreasePercent, int targetEntity)
    {
        Duration = duration;
        DamageInterval = damageInterval;
        DamageIncreasePercent = damageIncreasePercent;
        TargetEntity = targetEntity;
        DamageInInterval = damageInInterval;
        ElapsedTime = 0;
    }
}