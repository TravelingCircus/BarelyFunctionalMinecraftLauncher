using System.Net.Sockets;
using System.Text;
using CommonData;

namespace FileClient.Requests;

public class Request: Message
{
    public static void SendTextMessage(NetworkStream stream, string message)
    {
        byte[] requestBytes = Encoding.ASCII.GetBytes(message);
        stream.Write(requestBytes, 0, requestBytes.Length);
    }
}