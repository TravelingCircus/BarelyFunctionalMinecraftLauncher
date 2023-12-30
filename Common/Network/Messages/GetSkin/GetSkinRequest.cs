namespace Common.Network.Messages.GetSkin;

public sealed class GetSkinRequest: Message
{
    public string Nickname;

    public GetSkinRequest() { }
    
    public GetSkinRequest(string nickname)
    {
        Nickname = nickname;
    }

    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(GetSkinRequest)), Nickname.Length);
    }

    public override void ApplyData(Stream stream)
    {
        Nickname = StringReadStream(stream);
    }

    protected override Stream GetData()
    {
        MemoryStream buffer = new MemoryStream(GetHeader().DataLength);
        WriteToStream(buffer, Nickname);
            
        buffer.Flush();
        buffer.Position = 0;
        return buffer;
    }
}