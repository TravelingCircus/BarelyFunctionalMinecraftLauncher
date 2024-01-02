using System;
using System.IO;
using BFML.Repository;

namespace BFML.Core;

[Serializable]
public sealed class LocalPrefs
{
    internal string Nickname { get; set; }
    internal string PasswordHash { get; set; }
    
    internal int DedicatedRAM { get; set; }
    internal bool IsFullscreen { get; set; }
    
    internal string LastModPackGuid { get; set; }
    internal string LastForgeVersion { get; set; }
    internal string LastVanillaVersion { get; set; }
    
    internal LauncherMode LauncherMode { get; set; }
    internal DirectoryInfo GameDirectory { get; set; }
    internal FileValidation FileValidationMode { get; set; }

    internal LocalPrefs() : this(
        string.Empty, string.Empty,
        2048, false, string.Empty, 
        string.Empty, string.Empty, LauncherMode.Manual, null) { }

    internal LocalPrefs(
        string nickname, string passwordHash, int dedicatedRam, 
        bool isFullscreen, string lastModPackGuid, string lastForgeVersion, 
        string lastVanillaVersion, LauncherMode launcherMode, DirectoryInfo gameDirectory)
    {
        Nickname = nickname;
        PasswordHash = passwordHash;
        DedicatedRAM = dedicatedRam;
        IsFullscreen = isFullscreen;
        LastModPackGuid = lastModPackGuid;
        LastForgeVersion = lastForgeVersion;
        LastVanillaVersion = lastVanillaVersion;
        LauncherMode = launcherMode;
        GameDirectory = gameDirectory;
    }
}