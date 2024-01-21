using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BFML._3D;
using Common;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Utils;

namespace BFML.Repository.RepoIOAdapters;

internal sealed class ResourceAdapter : RepoAdapter
{
    private FileInfo DefaultSkinFile => new FileInfo(AdapterDirectory + "\\DefaultSkin.png");
    private FileInfo ShadowImageFile => new FileInfo(AdapterDirectory + "\\Shadow.png");
    private FileInfo VertexShaderFile => new FileInfo(AdapterDirectory + "\\Shader.vert");
    private FileInfo FragmentShaderFile => new FileInfo(AdapterDirectory + "\\Shader.frag");
    private FileInfo PlayerModelFile => new FileInfo(AdapterDirectory + "\\SkinPreview.obj");
    private FileInfo PlaneModelFile => new FileInfo(AdapterDirectory + "\\Plane.obj");
    
    internal ResourceAdapter(DirectoryInfo directory, RepoIO repoIo) : base(directory, repoIo) { }
    
    internal async Task<Skin> LoadDefaultSkin()
    {
        return new Skin(await LoadTexture(DefaultSkinFile));
    }
    
    internal Task<Texture> LoadShadowImage()
    {
        return LoadTexture(ShadowImageFile);
    }

    internal Task<Result<Shader>> LoadShader()
    {
        try
        {
            using StreamReader vertexReader = new StreamReader(VertexShaderFile.FullName, Encoding.UTF8);
            string vertexSource = vertexReader.ReadToEnd();

            using StreamReader fragmentReader = new StreamReader(FragmentShaderFile.FullName, Encoding.UTF8);
            string fragmentSource = fragmentReader.ReadToEnd();

            return Task.FromResult(Result<Shader>.Ok(new Shader(vertexSource, fragmentSource)));
        }
        catch (Exception e)
        {
            return Task.FromResult(Result<Shader>.Err(e));
        }
    }

    internal Task<Result<Model>> LoadPlayerModel()
    {
        return LoadObjModel(PlayerModelFile);
    }
    
    internal Task<Result<Model>> LoadPlaneModel()
    {
        return LoadObjModel(PlaneModelFile);
    }

    private Task<Result<Model>> LoadObjModel(FileInfo objFile)
    {
        try
        {
            ObjModelLoader skinModelLoader = new ObjModelLoader(objFile.FullName);
            Model model = skinModelLoader.Load();
            return Task.FromResult(Result<Model>.Ok(model));
        }
        catch (Exception e)
        {
            return Task.FromResult(Result<Model>.Err(e));
        }
    }
    
    private async Task<Texture> LoadTexture(FileInfo file)
    {
        Image<Rgba32> image = await Image.LoadAsync<Rgba32>(file.FullName);
        image.Mutate(x => x.Flip(FlipMode.Vertical));
        byte[] pixels = new byte[4 * image.Width * image.Height];
        image.CopyPixelDataTo(pixels);
        return new Texture(pixels, image.Width, image.Height);
    }
}