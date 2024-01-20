using System;
using Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BFML._3D;

public class SkinPreviewRenderer
{
    private Camera _camera;
    private RenderObject _skinPreviewRenderObject;
    private RenderObject _dustRenderObject;
    private bool _isInitialized;
    private bool _isFaulted;
    
    public void SetUp(Texture skinTexture, Texture shadowTexture)
    {
        try
        {
            _camera = new Camera(
                new Vector3(0f, -1.1f, -5f),
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
            Model skinModel = skinModelLoader.Load();
            Shader shader = new Shader("shader.vert", "shader.frag");
            Material skinMaterial = new Material(shader, skinTexture);
            _skinPreviewRenderObject = RenderObject.Instantiate(skinModel, skinMaterial);

            ObjModelLoader dustModelLoader = new ObjModelLoader("plane.obj");
            Model dustModel = dustModelLoader.Load();
            Material dustMaterial = new Material(shader, shadowTexture);
            _dustRenderObject = RenderObject.Instantiate(dustModel, dustMaterial);
        }
        catch (Exception e)
        {
            _isFaulted = true;
        }
        finally
        {
            _isInitialized = true;
        }
    }

    public void Render(float angle)
    {
        if (!_isInitialized || _isFaulted) return;

        try
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            angle += 155f;
            Quaternion nextRotation = Quaternion.FromEulerAngles(0f, MathHelper.DegreesToRadians(angle), 0f);
            _skinPreviewRenderObject.Transform.Rotation = nextRotation;
        
            _dustRenderObject.Render(_camera);
            _skinPreviewRenderObject.Render(_camera);
        }
        catch (Exception e)
        {
            _isFaulted = true;
        }
    }

    public void ChangeSkin(Skin skin)
    {
        _skinPreviewRenderObject.Material.Texture = skin.Texture;
    }
}