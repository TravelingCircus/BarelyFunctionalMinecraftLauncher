namespace TCPFileServer;

internal static class Program
{
    private static async Task Main()
    {
        Server server = new Server();
        server.Start();
        
        while (server.IsRunning)
        {
            await Task.Delay(100);
        }
    }
}