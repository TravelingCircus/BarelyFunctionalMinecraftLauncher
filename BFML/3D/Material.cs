using OpenTK.Graphics.OpenGL4;

namespace BFML._3D;

public sealed class Material
{
    public Texture Texture;
    public readonly Shader Shader;

    public Material(Shader shader, Texture texture)
    {
        Shader = shader;
        Texture = texture;
    }

    public void Use()
    {
        Shader.Use();
        Texture.Use(TextureUnit.Texture0);
    }
}