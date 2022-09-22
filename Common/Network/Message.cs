namespace CommonData.Network;

public abstract class Message
{
    public abstract MessageHeader GetHeader();

    public Task WriteDataTo(Stream targetStream)
    {
        return GetData().CopyToAsync(targetStream);
    }

    public abstract void FromData(Stream stream);

    protected abstract Stream GetData();
}