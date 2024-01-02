using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using BFML.Repository;

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
        string.Empty, string.Empty, LauncherMode.Manual, null) { }

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
    }

    public XmlSchema GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        reader.ReadToFollowing("Nickname");
        Nickname = reader.ReadElementString("Nickname");
        PasswordHash = reader.ReadElementString("PasswordHash");
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
        writer.WriteAttributeString("Nickname", Nickname);
        writer.WriteAttributeString("PasswordHash", PasswordHash);
        writer.WriteAttributeString("DedicatedRam", DedicatedRam.ToString());
        writer.WriteAttributeString("IsFullscreen", IsFullscreen.ToString());
        writer.WriteAttributeString("LauncherMode", LauncherMode.ToString());
        writer.WriteAttributeString("LastVanillaVersion", LastVanillaVersion);
        writer.WriteAttributeString("LastForgeVersion", LastForgeVersion);
        writer.WriteAttributeString("LastModPackGuid", LastModPackGuid);
        writer.WriteAttributeString("GameDirectory", GameDirectory.FullName);
        writer.WriteAttributeString("FileValidationMode", FileValidationMode.ToString());
    }
}