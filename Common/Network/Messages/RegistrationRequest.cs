using System.Text;

namespace CommonData.Network.Messages;

public class RegistrationRequest : Message
{
    public const byte Key = 1;
    public string NickName;
    public string PasswordHash;

    public RegistrationRequest()
    {
    }

    public RegistrationRequest(string nickName, string passwordHash)
    {
        NickName = nickName;
        PasswordHash = passwordHash;
    }

    public override MessageHeader GetHeader()
    {
        return new MessageHeader(Key, 
            2 * sizeof(int)
            +NickName.Length * sizeof(char)
            +PasswordHash.Length * sizeof(char));
    }

    protected override Stream GetData()
    {
        MemoryStream buffer = new MemoryStream();
        byte[] nicknameBytes = Encoding.UTF8.GetBytes(NickName);
        byte[] passwordBytes = Encoding.UTF8.GetBytes(PasswordHash);
        
        buffer.Write(BitConverter.GetBytes(nicknameBytes.Length));
        buffer.Write(nicknameBytes);
        buffer.Write(BitConverter.GetBytes(passwordBytes.Length));
        buffer.Write(passwordBytes);
        return buffer;
    }

    public override void FromData(Stream stream)
    {
        byte[] intBuffer = new byte[4];
        byte[] stringBuffer;
        //TODO extract writing/reading primitive types
        stream.Read(intBuffer, 0, 4);
        int stringLength = BitConverter.ToInt32(intBuffer, 0);
        stringBuffer = new byte[stringLength];
        stream.Read(stringBuffer, 0, stringLength);
        NickName = Encoding.UTF8.GetString(stringBuffer);
        
        stream.Read(intBuffer, 0, 4);
        stringLength = BitConverter.ToInt32(intBuffer, 0);
        stringBuffer = new byte[stringLength];
        stream.Read(stringBuffer, 0, stringLength);
        PasswordHash = Encoding.UTF8.GetString(stringBuffer);
    }
}