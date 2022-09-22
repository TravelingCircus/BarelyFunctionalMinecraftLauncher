using System.Text;

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

    protected String StringReadStream(Stream stream)
    {
        int stringLength = IntReadStream(stream);
        byte[] stringBuffer = new byte[stringLength];
        stream.Read(stringBuffer, 0, stringLength);
        return Encoding.UTF8.GetString(stringBuffer);
    }

    protected int IntReadStream(Stream stream)
    {
        byte[] intBuffer = new byte[4];
        stream.Read(intBuffer, 0, 4);
        return BitConverter.ToInt32(intBuffer, 0);
    }

    protected bool BoolReadStream(Stream stream)
    {
        byte[] byteBuffer = new byte[1];
        stream.Read(byteBuffer, 0, 1);
        return BitConverter.ToBoolean(byteBuffer);
    }

    protected void WriteStream(Stream stream, String value)
    {
        
    }

    protected void WriteStream(Stream stream, int value)
    {
        
    }

    protected void WriteStream(Stream stream, bool value)
    {
        
    }
}