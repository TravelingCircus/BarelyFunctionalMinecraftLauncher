using System;
using System.Threading.Tasks;
using BFML.Repository;
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
    private readonly Repo _repo;

    internal SkinPreviewRenderer(Repo repo)
    {
        _repo = repo;
    }

    public async Task SetUp()
    {
        try
        {
            SetUpGLState();
            
            _camera = new Camera(
                new Vector3(0f, -1.1f, -5f),
                Quaternion.Identity
            );
            
            Shader shader = (await _repo.LoadDefaultShader()).Unwrap();
            
            Model skinModel = (await _repo.LoadPlayerModel()).Unwrap();
            Material skinMaterial = new Material(shader, (await _repo.LoadDefaultSkin()).Texture);
            _skinPreviewRenderObject = RenderObject.Instantiate(skinModel, skinMaterial);
            
            Model dustModel = (await _repo.LoadPlaneModel()).Unwrap();
            Material dustMaterial = new Material(shader, await _repo.LoadShadowTexture());
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

    private void SetUpGLState()
    {
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back);
        float grey = 56f / 255f;
        GL.ClearColor(grey, grey, grey, 1f);
    }
}