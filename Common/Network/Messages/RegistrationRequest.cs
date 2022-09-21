using System.ComponentModel;
using System.Text;

namespace CommonData.Network.Messages;

public class RegistrationRequest : Message
{
    public readonly string NickName;
    public readonly string PasswordHash;

    public RegistrationRequest(string nickName, string passwordHash)
    {
        NickName = nickName;
        PasswordHash = passwordHash;
    }

    public override byte[] GetHeader()
    {
        throw new NotImplementedException();
    }

    public override Stream GetData()
    {
        MemoryStream buffer = new MemoryStream();
        buffer.Write(Encoding.UTF8.GetBytes(NickName));
        buffer.Write(Encoding.UTF8.GetBytes(PasswordHash));
        return buffer;
    }

    public override Message FromData(StreamReader reader)
    {
        throw new NotImplementedException();
    }
}