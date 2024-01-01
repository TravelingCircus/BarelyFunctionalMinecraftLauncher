using OpenTK.Graphics.OpenGL4;

namespace BFML._3D;

public sealed class Material
{
    public readonly Shader Shader;
    private Texture _texture;

    public Material(Shader shader, byte[] png)
    {
        Shader = shader;
        ChangeTexture(png);
    }

    public void ChangeTexture(byte[] png)
    {
        _texture = new Texture(png);
    }

    public void Use()
    {
        Shader.Use();
        _texture.Use(TextureUnit.Texture0);
    }
}