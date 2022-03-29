namespace DiplomaServer.World.Components;

public struct Projectile
{
    public int TargetId;
    public float Speed;
    public int Damage;

    public Projectile(int targetId, float speed, int damage)
    {
        TargetId = targetId;
        Speed = speed;
        Damage = damage;
    }
}