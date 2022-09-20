using System.Xml.Serialization;

namespace CommonData;

public static class UserDataSerializer
{
    public static void ToXML(User user, Stream buffer)
    {
        XmlSerializer xml = new XmlSerializer(typeof(User));
        xml.Serialize(buffer, user);
    }
    
    public static User FromXML(Stream stream)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(User));
        User user = (serializer.Deserialize(stream) as User)!;
        if (user is null) throw new InvalidDataException("Invalid user xml stream");
        return user;
    }
}