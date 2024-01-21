using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BFML._3D;
using Common;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

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
    
    internal Task<FileInfo> LoadFont()
    {
        throw new NotImplementedException();
    }
    
    internal async Task<Skin> LoadDefaultSkin()
    {
        return new Skin(await LoadTexture(DefaultSkinFile));
    }
    
    internal Task<Texture> LoadShadowImage()
    {
        return LoadTexture(ShadowImageFile);
    }

    internal Task<Shader> LoadShader()
    {
        try
        {
            string vertexShaderSource;
            string fragmentShaderSource;
            
            using (StreamReader reader = new StreamReader(VertexShaderFile.FullName, Encoding.UTF8))
            {
                vertexShaderSource = reader.ReadToEnd();
            }

            using (StreamReader reader = new StreamReader(FragmentShaderFile.FullName, Encoding.UTF8))
            {
                fragmentShaderSource = reader.ReadToEnd();
            }
            
            return Task.FromResult<Shader>(new Shader(vertexShaderSource, fragmentShaderSource));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
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