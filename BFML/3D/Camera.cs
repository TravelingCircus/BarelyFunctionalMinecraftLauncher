using OpenTK.Mathematics;

namespace BFML._3D;

public class Camera
{
    public readonly Vector3 Position;
    public readonly Quaternion Rotation;

    public Camera(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }

    public Matrix4 GetViewMatrix()
    {
        return Matrix4.CreateFromQuaternion(Rotation) 
               * Matrix4.CreateTranslation(Position);
    }
}