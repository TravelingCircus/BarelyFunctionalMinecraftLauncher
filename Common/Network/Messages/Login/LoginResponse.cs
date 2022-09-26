namespace CommonData.Network.Messages.Login;

public class LoginResponse: Message
{
    public bool Success;

    public LoginResponse()
    {
        
    }
    
    public LoginResponse(bool success)
    {
        Success = success;
    }

    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(LoginResponse)), 1);
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