using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Version;
using Common.Models;
using TCPFileClient.Utils;

namespace BFML.Core;

public sealed class Forge
{
    public readonly MVersion Version;
    private readonly MinecraftPath _minecraftPath;

    public Forge(MVersion version, MinecraftPath minecraftPath)
    {
        Version = version;
        _minecraftPath = minecraftPath;
    }

    public bool IsInstalled(LaunchConfiguration launchConfiguration)
    {
        string forgeVersionPath = _minecraftPath.Versions + $@"\{launchConfiguration.ForgeVersion}";
        return new DirectoryInfo(forgeVersionPath).Exists;
    }

    public Task Install(string archivePath, LaunchConfiguration launchConfiguration)
    {
        using TempDirectory tempDirectory = new TempDirectory();
        using (ZipArchive archive = ZipFile.OpenRead(archivePath))
        {
            archive.ExtractToDirectory(tempDirectory.Info.FullName);
        }


        InstallVersion(new DirectoryInfo(tempDirectory.Info.FullName + $"\\{launchConfiguration.ForgeVersion}"));
        InstallLibraries(new DirectoryInfo(tempDirectory.Info.FullName + "\\libraries"));
        
        return Task.CompletedTask;
    }

    private void InstallVersion(DirectoryInfo source)
    {
        if (!source.Exists) throw new DirectoryNotFoundException($"Target forge version isn't present in archive. Target: {source.Name}");

        if (!new DirectoryInfo(_minecraftPath.Versions).Exists) Directory.CreateDirectory(_minecraftPath.Versions);
        source.MergeTo(new DirectoryInfo(_minecraftPath.Versions+$"\\{source.Name}"));
    }
    
    private void InstallLibraries(DirectoryInfo source)
    {
        if (!source.Exists) throw new DirectoryNotFoundException("Libraries directory isn't present in archive. Proper name: libraries");

        if (!new DirectoryInfo(_minecraftPath.Library).Exists) Directory.CreateDirectory(_minecraftPath.Library);
        source.MergeTo(new DirectoryInfo(_minecraftPath.Library));
    }
}