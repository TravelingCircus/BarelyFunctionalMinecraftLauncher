using ABOBAEngine.Rendering.Models;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BFML._3D;

public class SkinPreviewRenderer
{
    private Camera _camera;
    private RenderObject _skinPreviewRenderObject;
    private RenderObject _dustRenderObject;
    
    public void SetUp()
    {
        _camera = new Camera(
            new Vector3(0f, 0f, -5f), 
            Quaternion.Identity
            );

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back);        
        float grey = 56f / 255f;
        GL.ClearColor(grey, grey, grey, 1f);
        
        ObjModelLoader skinModelLoader = new ObjModelLoader("SkinPreview.obj");
        Model skinModel =  skinModelLoader.Load();
        Shader shader = new Shader("shader.vert", "shader.frag");
        Material skinMaterial = new Material(shader, "van.png");
        _skinPreviewRenderObject = RenderObject.Instantiate(skinModel, skinMaterial);
        _skinPreviewRenderObject.Transform.Rotation = Quaternion.FromEulerAngles(0f, MathHelper.DegreesToRadians(155f), 0f);
        _skinPreviewRenderObject.Transform.Position = new Vector3(0f, -1.1f, 0f);
    }

    public void Render()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _skinPreviewRenderObject.Render(_camera);
    }

    public void ChangeSkin(string pngPath)
    {
        
    }
}