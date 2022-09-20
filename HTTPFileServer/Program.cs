using CommonData;

namespace HTTPFileServer;

internal static class Program
{
    private static Network network = new Network();
    
    private static void Main(string[] args)
    {   
        try
        {
            network.Start();

            while (true)
            {
                byte[] bytes = network.ListenForBytes();

                User user = (User)DataConverter.ByteArrayToObject(bytes);
                UserDataSerializer.SerializeXML(user);  
                Console.WriteLine("Sent request: ");
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
    
    private static byte[] GetByteArrayFromFile(String path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        return bytes;
    }
}