using System.Numerics;

namespace DiplomaServer.World.Components;

public struct Position
{
    public  Vector3 Value;

    public Position(Vector3 vector3)
    {
        Value = vector3;
    }
}