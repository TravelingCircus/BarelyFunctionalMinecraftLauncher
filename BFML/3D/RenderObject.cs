using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BFML._3D;

public class RenderObject : SceneObject
{
    public Material Material { get; private set; }
    private Model _model;

    private int _vertexArrayObject;     //this models space in V-RAM
    private int _verticesVBO;           //VertexBufferObject - array of vertices
    private int _albedoUVsVBO;          //array of texture UVs

    public static RenderObject Instantiate(Model model, Material material)
    {
        int vertexArrayObject = GL.GenVertexArray();
        int verticesVBO = GL.GenBuffer();
        int albedoUVsVBO = GL.GenBuffer();
        GL.BindVertexArray(vertexArrayObject);
        material.Use();
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, verticesVBO);
        GL.BufferData(BufferTarget.ArrayBuffer, model.Vertices.Length * sizeof(float), 
            model.Vertices, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, albedoUVsVBO);
        GL.BufferData(BufferTarget.ArrayBuffer, model.AlbedoMapUVs.Length * sizeof(float), 
            model.AlbedoMapUVs, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(1);
        
        GL.BindVertexArray(0);
        return new RenderObject
        {
            _vertexArrayObject = vertexArrayObject,
            _albedoUVsVBO = albedoUVsVBO,
            _verticesVBO = verticesVBO,
            Material = material,
            _model = model
        };
    }
    
    public void Render(Camera camera)
    {
        GL.BindVertexArray(_vertexArrayObject);
        Material.Use();

        Matrix4 model = Matrix4.CreateFromQuaternion(Transform.Rotation) * Matrix4.CreateTranslation(Transform.Position);
        Matrix4 view = camera.GetViewMatrix();
        Matrix4 projection =
            Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), 350f / 550f, 0.1f, 100.0f);
        
        Matrix4 transform = model * view * projection;
        Material.Shader.SetUniformMatrix4("transform", ref transform);
        
        GL.DrawArrays(PrimitiveType.Triangles, 0, _model.Vertices.Length/3);
    }

    public void DisposeOf()
    {
        GL.DeleteBuffer(_verticesVBO);
        GL.DeleteBuffer(_albedoUVsVBO);
        Material.Shader.Dispose();
    }
}