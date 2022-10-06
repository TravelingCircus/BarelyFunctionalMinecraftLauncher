using OpenTK.Mathematics;

namespace ABOBAEngine;

public class Transform
{
    public Vector3 Position = Vector3.Zero;
    public Quaternion Rotation = Quaternion.Identity;
    public Vector3 Scale = Vector3.One;
}