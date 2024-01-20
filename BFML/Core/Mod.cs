using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BFML.Core;

public sealed class Mod : IXmlSerializable
{
    internal string Name { get; private set; }
    internal string Version { get; private set; }
    internal bool IsLibrary { get; private set; }
    internal ModDependency[] Dependencies { get; private set; }
    
    [Serializable]
    public sealed class ModDependency
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
        IsLibrary = bool.Parse(reader.ReadElementString("IsLibrary"));
        
        List<ModDependency> modsDependencies = new List<ModDependency>();
        XmlSerializer dependencySerializer = new XmlSerializer(typeof(ModDependency));

        while (reader.ReadToFollowing("Dependency"))
        {
            modsDependencies.Add(dependencySerializer.Deserialize(reader) as ModDependency);
        }

        Dependencies = modsDependencies.ToArray();
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteElementString("Name", Name);
        writer.WriteElementString("Version", Version);
        writer.WriteElementString("IsLibrary", IsLibrary.ToString());
        writer.WriteStartElement("Dependencies");
        writer.WriteValue(Dependencies); //TODO test it
        writer.WriteEndElement();
    }
}