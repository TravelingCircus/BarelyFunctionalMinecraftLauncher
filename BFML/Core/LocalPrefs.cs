using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using BFML.Repository;
using CmlLib.Core;
using CmlLib.Core.Java;

namespace BFML.Core;

public sealed class LocalPrefs : IXmlSerializable
{
    internal string Nickname { get; set; }
    internal string PasswordHash { get; set; }
    
    internal int DedicatedRam { get; set; }
    internal bool IsFullscreen { get; set; }
    internal bool ShowSnapshots { get; set; }
    
    internal string LastModPackGuid { get; set; }
    internal string LastForgeVersion { get; set; }
    internal string LastVanillaVersion { get; set; }
    
    internal LauncherMode LauncherMode { get; set; }
    internal DirectoryInfo GameDirectory { get; set; }
    internal FileInfo JVMLocation { get; set; }
    internal FileValidation FileValidationMode { get; set; }

    internal LocalPrefs() { }

    internal LocalPrefs(
        string nickname, string passwordHash, int dedicatedRam, bool isFullscreen, bool showSnapshots, 
        string lastVanillaVersion, string lastForgeVersion, string lastModPackGuid,
        LauncherMode launcherMode, DirectoryInfo gameDirectory, FileInfo jvmLocation)
    {
        Nickname = nickname;
        PasswordHash = passwordHash;
        DedicatedRam = dedicatedRam;
        IsFullscreen = isFullscreen;
        ShowSnapshots = showSnapshots;
        LastModPackGuid = lastModPackGuid;
        LastForgeVersion = lastForgeVersion;
        LastVanillaVersion = lastVanillaVersion;
        LauncherMode = launcherMode;
        GameDirectory = gameDirectory;
        JVMLocation = jvmLocation;
    }

    internal static LocalPrefs Default()
    {
        return new LocalPrefs(
            string.Empty, string.Empty,
            2048, false, false,
            string.Empty, string.Empty, string.Empty,
            LauncherMode.Manual, new DirectoryInfo(MinecraftPath.WindowsDefaultPath), null); //Provide default jvm path
    }

    public XmlSchema GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        reader.ReadToFollowing("Nickname");
        Nickname = reader.ReadElementString("Nickname");
        if (string.IsNullOrEmpty(Nickname)) Nickname = "Steve";
        
        PasswordHash = reader.ReadElementString("PasswordHash");
        DedicatedRam = int.Parse(reader.ReadElementString("DedicatedRam"));
        IsFullscreen = bool.Parse(reader.ReadElementString("IsFullscreen"));
        ShowSnapshots = bool.Parse(reader.ReadElementString("ShowSnapshots"));
        LauncherMode = Enum.Parse<LauncherMode>(reader.ReadElementString("LauncherMode"));
        LastVanillaVersion = reader.ReadElementString("LastVanillaVersion");
        LastForgeVersion = reader.ReadElementString("LastForgeVersion");
        LastModPackGuid = reader.ReadElementString("LastModPackGuid");
        GameDirectory = new DirectoryInfo(reader.ReadElementString("GameDirectory"));
        
        string jvmPath = reader.ReadElementString("JVMLocation");
        
        jvmPath = String.IsNullOrWhiteSpace(jvmPath) 
            ? new CMLauncher(GameDirectory.FullName).GetDefaultJavaPath()! 
            : jvmPath;
        JVMLocation = new FileInfo(jvmPath);
        
        FileValidationMode = Enum.Parse<FileValidation>(reader.ReadElementString("FileValidationMode"));
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteElementString("Nickname", Nickname);
        writer.WriteElementString("PasswordHash", PasswordHash);
        writer.WriteElementString("DedicatedRam", DedicatedRam.ToString());
        writer.WriteElementString("IsFullscreen", IsFullscreen.ToString());
        writer.WriteElementString("ShowSnapshots", ShowSnapshots.ToString());
        writer.WriteElementString("LauncherMode", LauncherMode.ToString());
        writer.WriteElementString("LastVanillaVersion", LastVanillaVersion);
        writer.WriteElementString("LastForgeVersion", LastForgeVersion);
        writer.WriteElementString("LastModPackGuid", LastModPackGuid);
        writer.WriteElementString("GameDirectory", GameDirectory.FullName);
        writer.WriteElementString("JVMLocation", JVMLocation.FullName);
        writer.WriteElementString("FileValidationMode", FileValidationMode.ToString());
    }
}