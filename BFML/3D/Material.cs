using OpenTK.Graphics.OpenGL4;

namespace BFML._3D;

public class Material
{
    public readonly Shader Shader;
    private readonly Texture _texture;

    public Material(Shader shader, string texturePath)
    {
        Shader = shader;
        _texture = new Texture(texturePath);
    }

    public virtual void Use()
    {
        Shader.Use();
        _texture.Use(TextureUnit.Texture0);
    }
}