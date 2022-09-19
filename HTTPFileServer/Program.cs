using System.Net;
using System.Net.Sockets;
using CommonData;

namespace HTTPFileServer;

internal static class Program
{
    private static void Main(string[] args)
    {
        try
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Any, 7000);
            Console.WriteLine("Server started");
            serverSocket.Start();
            TcpClient clientSocket;
            NetworkStream clientStream;

            while (true)
            {
                clientSocket = serverSocket.AcceptTcpClient();
                clientStream = clientSocket.GetStream();

                //byte[] bytes = GetByteArrayFromFile(@"D:\Home\Desktope\ExtractedForgeFiles1.16.5.zip");
                byte[] bytes = DataConverter.ReadAllBytesFromStream(clientStream);
                User user = (User) DataConverter.ByteArrayToObject(bytes);
                UserDataSerializer.SerializeXML(user);
                /*clientStream.Write(bytes, 0, bytes.Length);
                clientStream.Flush();
                Console.WriteLine("Sent request: ");*/

                clientSocket.Close();
            }
            
            serverSocket.Stop();
            Console.WriteLine("Server stopped");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private static byte[] GetByteArrayFromFile(String path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        return bytes;
    }
}