using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Version = Utils.Version;

namespace BFML.Core;

public sealed class ModPack : IXmlSerializable
{
    internal Guid Guid { get; private set; }
    internal string Name { get; private set; }
    internal Version VanillaVersion { get; private set; }
    internal Version ForgeVersion { get; private set; }
    internal Version ModPackVersion { get; private set; }
    internal string Changelog { get; private set; }
    internal ulong ModsChecksum { get; private set; }
    internal Mod[] Mods { get; private set; }
    
    internal ModPack() { }

    public XmlSchema GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        reader.ReadToFollowing("Guid");
        Guid = Guid.Parse(reader.ReadElementString("Guid"));
        Name = reader.ReadElementString("Name");
        VanillaVersion = new Version(reader.ReadElementString("VanillaVersion"));
        ForgeVersion = new Version(reader.ReadElementString("ForgeVersion"));
        ModPackVersion = new Version(reader.ReadElementString("ModPackVersion"));
        Changelog = reader.ReadElementString("Changelog");
        ModsChecksum = ulong.Parse(reader.ReadElementString("ModsChecksum"));
        
        List<Mod> mods = new List<Mod>();
        XmlSerializer modSerializer = new XmlSerializer(typeof(Mod));

        while (reader.ReadToFollowing("Mod"))
        {
            mods.Add(modSerializer.Deserialize(reader) as Mod);
        }

        Mods = mods.ToArray();
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteElementString("Guid", Guid.ToString());
        writer.WriteElementString("Name", Name);
        writer.WriteElementString("VanillaVersion", VanillaVersion.ToString());
        writer.WriteElementString("ForgeVersion", ForgeVersion.ToString());
        writer.WriteElementString("ModPackVersion", ModPackVersion.ToString());
        writer.WriteElementString("Changelog", Changelog);
        writer.WriteElementString("ModsChecksum", ModsChecksum.ToString());
        writer.WriteStartElement("Mods");
        writer.WriteValue(Mods); //TODO test it
        writer.WriteEndElement();
    }
}
