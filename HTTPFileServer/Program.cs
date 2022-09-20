namespace HTTPFileServer;

internal static class Program
{
    private static Network network = new Network();
    private static event Action<byte[]> ReceivedBytes;
    
    private static void Main(string[] args)
    {   
        try
        {
            network.Start();
            while (true)
            {
                byte[] bytes = network.ListenForBytes();
                ReceivedBytes?.Invoke(bytes);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            network.Dispose();
        }
    }
}