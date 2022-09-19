using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace CommonData;

public class DataConverter
{
    public static byte[] ReadAllBytesFromStream(NetworkStream networkStream)
    {
        byte[] data = new byte[1024];
        byte[] buffer;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            int read;
            while ((read = networkStream.Read(data, 0, data.Length)) > 0)
            {
                memoryStream.Write(data, 0, read);
            }

            buffer = memoryStream.ToArray();
        }

        return buffer;
    }
    
    public static byte[] ObjectToByteArray(object obj)
    {
        if(obj is null)
            return null;
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (MemoryStream memoryStream = new MemoryStream())
        {
            binaryFormatter.Serialize(memoryStream, obj);
            return memoryStream.ToArray();
        }
    }
    
    public static object ByteArrayToObject(byte[] byteArray)
    {
        if(byteArray is null)
            return null;
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (MemoryStream memoryStream = new MemoryStream(byteArray))
        {
            return binaryFormatter.Deserialize(memoryStream);
        }
    }
}