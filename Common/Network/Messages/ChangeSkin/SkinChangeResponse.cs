namespace Common.Network.Messages.ChangeSkin;

public class SkinChangeResponse: Message
{
    public bool Success;

    public SkinChangeResponse() { }
    
    public SkinChangeResponse(bool success)
    {
        Success = success;
    }

    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(SkinChangeResponse)), 1);
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
            
        buffer.Flush();
        buffer.Position = 0;
        return buffer;
    }
}