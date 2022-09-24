namespace HTTPFileServer.DataAccess;

public class SmallDataHandler: DataHandler
{
    public override Task WriteToRepository()
    {
        throw new NotImplementedException();
    }

    public override byte[] ReadFromRepository()
    {
        throw new NotImplementedException();
    }
}