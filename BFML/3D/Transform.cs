using OpenTK.Mathematics;

namespace BFML._3D;

public class Transform
{
    public Vector3 Scale = Vector3.One;
    public Vector3 Position = Vector3.Zero;
    public Quaternion Rotation = Quaternion.Identity;
}