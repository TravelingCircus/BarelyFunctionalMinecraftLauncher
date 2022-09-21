namespace CommonData;

public abstract class Message
{
    public abstract byte[] GetHeader();

    public byte[] WriteDataTo(StreamWriter writer)
    {
        //TODO write from data stream to supplied writer
        throw new NotImplementedException();
    }

    public abstract StreamReader GetData();
}