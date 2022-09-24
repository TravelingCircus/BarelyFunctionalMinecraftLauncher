namespace HTTPFileServer.DataAccess;

public abstract class DataHandler
{
    public abstract Task WriteToRepository();
    
    public abstract byte[] ReadFromRepository();
}