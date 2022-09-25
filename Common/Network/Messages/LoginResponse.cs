namespace CommonData.Network.Messages;

public class LoginResponse: Message
{
    public bool LoginSuccess;

    public LoginResponse()
    {
        
    }
    
    public LoginResponse(bool loginSuccess)
    {
        LoginSuccess = loginSuccess;
    }

    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(LoginResponse)), 1);
    }

    public override void ApplyData(Stream stream)
    {
        LoginSuccess = BoolReadStream(stream);
    }

    protected override Stream GetData()
    {
        MemoryStream buffer = new MemoryStream(1);
        byte[] successBytes = BitConverter.GetBytes(LoginSuccess);
        buffer.Write(successBytes);
        return buffer;
    }
}