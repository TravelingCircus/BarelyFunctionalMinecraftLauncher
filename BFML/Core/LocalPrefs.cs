using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using BFML.Repository;
using CmlLib.Core;
using CmlLib.Core.Version;

namespace BFML.Core;

public sealed class LocalPrefs : IXmlSerializable
{
    internal string Nickname { get; set; }
    internal string PasswordHash { get; set; }
    
    internal int DedicatedRam { get; set; }
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
        string.Empty, string.Empty, 
        LauncherMode.Manual, new DirectoryInfo(MinecraftPath.WindowsDefaultPath)) { }

    internal LocalPrefs(
        string nickname, string passwordHash, int dedicatedRam, 
        bool isFullscreen, string lastModPackGuid, string lastForgeVersion, 
        string lastVanillaVersion, LauncherMode launcherMode, DirectoryInfo gameDirectory)
    {
        Nickname = nickname;
        PasswordHash = passwordHash;
        DedicatedRam = dedicatedRam;
        IsFullscreen = isFullscreen;
        LastModPackGuid = lastModPackGuid;
        LastForgeVersion = lastForgeVersion;
        LastVanillaVersion = lastVanillaVersion;
        LauncherMode = launcherMode;
        GameDirectory = gameDirectory;
    }

    public XmlSchema GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        reader.ReadToFollowing("Nickname");
        Nickname = reader.ReadElementString("Nickname");
        PasswordHash = reader.ReadElementString("PasswordHash");
        if (string.IsNullOrEmpty(PasswordHash)) PasswordHash = "Steve";
        DedicatedRam = int.Parse(reader.ReadElementString("DedicatedRam"));
        IsFullscreen = bool.Parse(reader.ReadElementString("IsFullscreen"));
        LauncherMode = Enum.Parse<LauncherMode>(reader.ReadElementString("LauncherMode"));
        LastVanillaVersion = reader.ReadElementString("LastVanillaVersion");
        LastForgeVersion = reader.ReadElementString("LastForgeVersion");
        LastModPackGuid = reader.ReadElementString("LastModPackGuid");
        GameDirectory = new DirectoryInfo(reader.ReadElementString("GameDirectory"));
        FileValidationMode = Enum.Parse<FileValidation>(reader.ReadElementString("FileValidationMode"));
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteElementString("Nickname", Nickname);
        writer.WriteElementString("PasswordHash", PasswordHash);
        writer.WriteElementString("DedicatedRam", DedicatedRam.ToString());
        writer.WriteElementString("IsFullscreen", IsFullscreen.ToString());
        writer.WriteElementString("LauncherMode", LauncherMode.ToString());
        writer.WriteElementString("LastVanillaVersion", LastVanillaVersion);
        writer.WriteElementString("LastForgeVersion", LastForgeVersion);
        writer.WriteElementString("LastModPackGuid", LastModPackGuid);
        writer.WriteElementString("GameDirectory", GameDirectory.FullName);
        writer.WriteElementString("FileValidationMode", FileValidationMode.ToString());
    }
}