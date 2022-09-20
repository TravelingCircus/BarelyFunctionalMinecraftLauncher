using System.Xml.Serialization;
using CommonData;

namespace HTTPFileServer;

public sealed class UserDataSerializer
{
    public static Task SerializeXML(User user)
    {
        XmlSerializer xml = new XmlSerializer(typeof(User));

        using (FileStream fileStream = new FileStream(@"D:\Home\Desktope\TestDownload\" 
                                                      + user.Nickname + ".xml", FileMode.OpenOrCreate))
        {
            xml.Serialize(fileStream, user);
        }
        return Task.CompletedTask;
    }
    
    public static Task<User> DeserializeXML(string nickname)
    {
        XmlSerializer xml = new XmlSerializer(typeof(User));

        using (FileStream fileStream = new FileStream(@"D:\Home\Desktope\TestDownload\" 
                                                      + nickname + ".xml", FileMode.OpenOrCreate))
        {
            Task<User> user = (Task<User>)xml.Deserialize(fileStream);
            return user;
        }
    }
}