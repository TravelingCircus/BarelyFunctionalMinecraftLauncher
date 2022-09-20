
namespace HTTPFileServer;

internal static class Program
{
    private static void Main(string[] args)
    {
        Server server = new Server();
        server.Start();
        if (Console.ReadLine() == "/close")
        {
            server.Terminate();
        }
    }
}