namespace Common.Network.Messages;

public abstract class ZipFileMessage: Message
{
    public abstract int GetDataLength();
    protected abstract Stream GetDataStream();
    public override async Task WriteDataTo(Stream targetStream)
    {
        int dataLength = GetDataLength();
        Stream dataStream = GetDataStream();
        WriteToStream(targetStream, dataLength);
        byte[] buffer = new byte[16777216];
        int lastRead;
        do 
        {
            lastRead = await dataStream.ReadAsync(buffer, 0, buffer.Length);
            await targetStream.WriteAsync(buffer, 0, lastRead);
        } while (lastRead >= buffer.Length);
        await dataStream.FlushAsync();
        dataStream.Close();
    }
}