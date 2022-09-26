using System.Text;

namespace CommonData.Network.Messages.Login;

public class LoginRequest: Message
{
    public string NickName;
    public string PasswordHash;

    public LoginRequest()
    {
        
    }

    public LoginRequest(string nickName, string passwordHash)
    {
        NickName = nickName;
        PasswordHash = passwordHash;
    }
    
    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(LoginRequest)), 
            2 * sizeof(int)
            +NickName.Length
            +PasswordHash.Length);
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
        buffer.Position = 0;
        return buffer;
    }

    public override void ApplyData(Stream stream)
    {
        NickName = StringReadStream(stream);
        PasswordHash = StringReadStream(stream);
    }
}