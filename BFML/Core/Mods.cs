using System;
using System.IO.Compression;
using System.Threading.Tasks;
using CmlLib.Core;
using TCPFileClient.Utils;

namespace BFML.Core;

public class Mods
{
    private readonly MinecraftPath _minecraftPath;

    public Mods(MinecraftPath minecraftPath)
    {
        _minecraftPath = minecraftPath;
    }

    public bool ChecksumMatches(string checksum)
    {
        return true;
        throw new NotImplementedException();
    }

    public Task InstallFromArchive(string archivePath)
    {
        using TempDirectory tempDirectory = new TempDirectory();
        using (ZipArchive archive = ZipFile.OpenRead(archivePath))
        {
            archive.ExtractToDirectory(_minecraftPath.BasePath + @"\mods");
        }
        return Task.CompletedTask;
    }
}