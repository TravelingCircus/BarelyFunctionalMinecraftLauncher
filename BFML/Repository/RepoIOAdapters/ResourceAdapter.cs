using System;
using System.IO;
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
    
    internal ResourceAdapter(DirectoryInfo directory, RepoIO repoIo) : base(directory, repoIo) { }
    
    internal Task<FileInfo> LoadFont()
    {
        throw new NotImplementedException();
    }
    
    internal async Task<Skin> LoadDefaultSkin()
    {
        Image<Rgba32> image = await Image.LoadAsync<Rgba32>(DefaultSkinFile.FullName);
        image.Mutate(x => x.Flip(FlipMode.Vertical));
        byte[] pixels = new byte[4 * image.Width * image.Height];
        image.CopyPixelDataTo(pixels);
        Texture texture = new Texture(pixels, image.Width, image.Height);
        return new Skin(texture);
    }
}