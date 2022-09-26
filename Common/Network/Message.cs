using System.Text;

namespace CommonData.Network;

public abstract class Message
{
    public abstract MessageHeader GetHeader();

    public Task WriteDataTo(Stream targetStream)
    {
        using Stream source = GetData();
        source.Flush();
        source.Position = 0;
        byte[] buffer = new byte[source.Length];
        source.Read(buffer, 0, buffer.Length);
        return targetStream.WriteAsync(buffer, 0, buffer.Length);
    }

    public abstract void ApplyData(Stream stream);

    protected abstract Stream GetData();
    
    protected byte[] ByteArrayReadStream(Stream stream, int arrayLength)
    {
        byte[] bytes = new byte[arrayLength];
        stream.Read(bytes, 0, arrayLength);
        return bytes;
    }

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

    protected void WriteToStream(Stream stream, string value)
    {
        byte[] stringBytes = Encoding.UTF8.GetBytes(value);
        stream.Write(BitConverter.GetBytes(stringBytes.Length));
        stream.Write(stringBytes);
    }

    protected void WriteToStream(Stream stream, int value)
    {
        stream.Write(BitConverter.GetBytes(value));
    }

    protected void WriteToStream(Stream stream, bool value)
    {
        stream.Write(BitConverter.GetBytes(value));
    }
}