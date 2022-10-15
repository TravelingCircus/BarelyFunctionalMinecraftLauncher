namespace Common.Network.Messages.GetSkin;

public sealed class GetSkinResponse: Message
{
    public int SkinDataLength;
    public byte[] SkinData;

    public GetSkinResponse()
    {
        
    }

    public GetSkinResponse(int skinDataLength, byte[] skinData)
    {
        SkinDataLength = skinDataLength;
        SkinData = skinData;
    }

    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(GetSkinResponse)), 
            sizeof(int) + SkinDataLength);
    }

    public override void ApplyData(Stream stream)
    {
        SkinDataLength = IntReadStream(stream);
        SkinData = ByteArrayReadStream(stream, SkinDataLength);
    }

    protected override Stream GetData()
    {
        MemoryStream buffer = new MemoryStream(GetHeader().DataLength);
        WriteToStream(buffer, SkinDataLength);
        buffer.Write(SkinData);
        
        buffer.Flush();
        buffer.Position = 0;
        return buffer;
    }
}