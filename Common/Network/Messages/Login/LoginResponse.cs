using CommonData.Models;

namespace CommonData.Network.Messages.Login;

public class LoginResponse : Message
{
    public bool Success;
    public User User;
    private byte[] SkinData;

    public LoginResponse()
    {
    }

    public LoginResponse(bool success, User user, byte[] skinData = null!)
    {
        Success = success;
        User = user;
        SkinData = skinData;
    }

    public override MessageHeader GetHeader()
    {
        int skinDataLength = SkinData is null ? 0 : SkinData.Length;
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(LoginResponse)), 
            1);
    }

    public override void ApplyData(Stream stream)
    {
        Success = BoolReadStream(stream);
    }

    protected override Stream GetData()
    {
        MemoryStream buffer = new MemoryStream(1);
        byte[] successBytes = BitConverter.GetBytes(Success);
        buffer.Write(successBytes);
        return buffer;
    }
}