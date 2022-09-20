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
        if(obj is null) throw new ArgumentNullException("obj");
        
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        byte[] result;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            binaryFormatter.Serialize(memoryStream, obj);
            result = memoryStream.ToArray();
        }
        return result;
    }
    
    public static T ByteArrayToObject<T>(byte[] byteArray)
    {
        if (byteArray is null || byteArray.Length < 1) throw new ArgumentNullException("byteArray", "input array is null or empty");
        
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using MemoryStream memoryStream = new MemoryStream(byteArray);
        return (T) binaryFormatter.Deserialize(memoryStream);
    }
}