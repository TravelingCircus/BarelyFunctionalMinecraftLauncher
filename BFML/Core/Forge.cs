using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Utils;

namespace BFML.Core;

internal sealed class Forge : IXmlSerializable
{
    internal string Name { get; private set; }
    internal Version TargetVanillaVersion { get; private set; }
    internal Version SubVersion { get; private set; }
    internal bool HasJarFile { get; private set; }
    
    public XmlSchema GetSchema() => null; //TODO Implement Serialization

    public void ReadXml(XmlReader reader)
    {
        throw new System.NotImplementedException();
    }

    public void WriteXml(XmlWriter writer)
    {
        throw new System.NotImplementedException();
    }
}