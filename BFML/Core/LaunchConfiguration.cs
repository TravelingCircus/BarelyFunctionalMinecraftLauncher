using System.IO;
using CmlLib.Core.Version;
using Utils;

namespace BFML.Core;

internal readonly struct LaunchConfiguration
{
    internal readonly MVersion Version;
    internal readonly int DedicatedRam;
    internal readonly bool FullScreen;
    internal readonly string Nickname;
    internal readonly FileInfo JVMLocation;

    public LaunchConfiguration(
        string nickname, 
        MVersion version, 
        int dedicatedRam, 
        bool fullScreen, FileInfo jvmLocation) : this()
    {
        Nickname = nickname;
        Version = version;
        DedicatedRam = dedicatedRam;
        FullScreen = fullScreen;
        JVMLocation = jvmLocation;
    }

    public Result<bool> IsValid()
    {
        if (DedicatedRam < 1024) return Result<bool>.Err(new InvalidDataException("Can't launch with less than 1GB of RAM"));
        if (string.IsNullOrEmpty(Nickname)) return Result<bool>.Err(new InvalidDataException("Can't launch with empty nickname"));
        
        return Result<bool>.Ok(true);
    }
}