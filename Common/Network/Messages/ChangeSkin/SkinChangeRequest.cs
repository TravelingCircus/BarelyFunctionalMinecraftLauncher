namespace Common.Network.Messages.ChangeSkin;

public class SkinChangeRequest: Message
{
    public string Nickname;
    public int SkinDataLength;
    public byte[] SkinData;

    public SkinChangeRequest()
    {
        
    }

    public SkinChangeRequest(string nickname, byte[] skinData, int skinDataLength)
    {
        Nickname = nickname;
        SkinData = skinData;
        SkinDataLength = skinDataLength;
    }

    public override MessageHeader GetHeader()
    {
        return new MessageHeader(MessageRegistry.GetKeyForMessageType(typeof(SkinChangeRequest)),
            Nickname.Length + sizeof(int) + SkinDataLength);
    }

    public override void ApplyData(Stream stream)
    {
        Nickname = StringReadStream(stream);
        SkinDataLength = IntReadStream(stream);
        SkinData = ByteArrayReadStream(stream, SkinDataLength);
    }

    protected override Stream GetData()
    {
        MemoryStream buffer = new MemoryStream(GetHeader().DataLength);
        WriteToStream(buffer, Nickname);
        WriteToStream(buffer, SkinDataLength);
        buffer.Write(SkinData);
        
        buffer.Flush();
        buffer.Position = 0;
        return buffer;
    }
}