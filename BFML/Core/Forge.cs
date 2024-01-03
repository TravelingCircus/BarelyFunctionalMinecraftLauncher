using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Utils;

namespace BFML.Core;

public sealed class Forge : IXmlSerializable //Has to be public for serialization
{
    internal string Name { get; private set; }
    internal Version TargetVanillaVersion { get; private set; }
    internal Version SubVersion { get; private set; }
    internal bool HasJarFile { get; private set; }
    
    public XmlSchema GetSchema() => null; //TODO Implement Serialization

    public void ReadXml(XmlReader reader)
    {
        reader.ReadToFollowing("Name");
        Name = reader.ReadElementString("Name");
        TargetVanillaVersion = new Version(reader.ReadElementString("Vanilla"));
        SubVersion = new Version(reader.ReadElementString("SubVersion"));
        HasJarFile = bool.Parse(reader.ReadElementString("HasJarFile"));
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteElementString("Name", Name);
        writer.WriteElementString("Vanilla", TargetVanillaVersion.ToString());
        writer.WriteElementString("SubVersion", SubVersion.ToString());
        writer.WriteElementString("HasJarFile", HasJarFile.ToString());
    }
}