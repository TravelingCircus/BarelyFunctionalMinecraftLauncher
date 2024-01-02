using System.Xml.Serialization;
using Common.Models;

namespace TCPFileServer.DataAccess;

public static class DataSerializer
{
    public static void UserToXml(User user, Stream buffer)
    {
        XmlSerializer xml = new XmlSerializer(typeof(User));
        xml.Serialize(buffer, user);
    }
    
    public static User UserFromXml(Stream stream)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(User));
        User user = (serializer.Deserialize(stream) as User)!;
        if (user is null) throw new InvalidDataException("Invalid user xml stream");
        return user;
    }
}