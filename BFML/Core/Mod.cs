using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BFML.Core;

internal sealed class Mod : IXmlSerializable
{
    internal string Name { get; private set; }
    internal string Version { get; private set; }
    internal bool IsLibrary { get; private set; }
    internal ModDependency[] Dependencies { get; private set; }
    
    internal sealed class ModDependency
    {
        public string Name;
        public string Version;
    }

    public XmlSchema GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        reader.ReadToFollowing("Name");
        Name = reader.ReadElementString("Name");
        Version = reader.ReadElementString("Version");
        IsLibrary = bool.Parse(reader.ReadElementString("InLibrary"));
        Dependencies = reader.ReadElementContentAs(typeof(ModDependency[]), null) as ModDependency[];
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteElementString("Name", Name);
        writer.WriteElementString("Version", Version);
        writer.WriteElementString("IsLibrary", IsLibrary.ToString());
        writer.WriteStartElement("Dependencies");
        writer.WriteValue(Dependencies);
        writer.WriteEndElement();
    }
}