using System;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BFML._3D;

public class SkinPreviewRenderer
{
    private Camera _camera;
    private readonly Shader _shader;
    private readonly Texture _texture;

    private int _vertexArrayObject;
    
    private readonly float[] _vertices =
    {
        0.5f, 0.5f, 0.0f, // top right
        0.5f, -0.5f, 0.0f, // bottom right
        -0.5f, -0.5f, 0.0f, // bottom left
        -0.5f, 0.5f, 0.0f // top left
    };

    private readonly uint[] _triangles =
    {
        0, 1, 3,
        1, 2, 3
    };

    private readonly float[] _uvs =
    {
        1, 1,
        1, 0,
        0, 0,
        0, 1
    };

    public SkinPreviewRenderer()
    {
        _camera = new Camera(-Vector3.UnitZ * 30f, Quaternion.Identity);
        _shader = new Shader("shader.vert", "shader.frag");
        _texture = new Texture("perfection.png");
    }

    public void SetUp()
    {
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Front);

        _vertexArrayObject = GL.GenVertexArray();
        int elementBufferObject = GL.GenBuffer();
        int verticesVBO = GL.GenBuffer();
        int albedoUVsVBO = GL.GenBuffer();

        GL.BindVertexArray(_vertexArrayObject);
        _shader.Use();
        _texture.Use(TextureUnit.Texture0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, verticesVBO);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
            BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _vertices.Length * sizeof(uint),
            _triangles, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ArrayBuffer, albedoUVsVBO);
        GL.BufferData(BufferTarget.ArrayBuffer, _uvs.Length * sizeof(float), _uvs,
            BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(1);

        GL.BindVertexArray(0);
    }

    public Task Render(TimeSpan deltaTime)
    {
        GL.BindVertexArray(_vertexArrayObject);
        _shader.Use();

        _camera.Position += Vector3.UnitZ * (deltaTime.Milliseconds / 50f) * (float)(Math.Pow(_camera.Position.Z/31f, 2));
        
        Matrix4 model = Matrix4.Identity;
        Matrix4 view = _camera.GetViewMatrix();
        Matrix4 projection =
            Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), 400f / 700f, 0.1f, 100.0f);
        Matrix4 transform = model * view * projection;
        _shader.SetUniformMatrix4("transform", ref transform);

        GL.DrawElements(PrimitiveType.Triangles, _triangles.Length, DrawElementsType.UnsignedInt, 0);

        return Task.CompletedTask;
    }
}