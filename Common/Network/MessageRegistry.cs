using System.ComponentModel;
using CommonData.Network.Messages;
using CommonData.Network.Messages.Login;
using CommonData.Network.Messages.Registration;

namespace CommonData.Network;

public static class MessageRegistry
{
    private static readonly Dictionary<byte, Type> _messages = new Dictionary<byte, Type>()
    {
        {1, typeof(RegistrationRequest)},
        {2, typeof(RegistrationResponse)},
        {3, typeof(LoginRequest)},
        {4, typeof(LoginResponse)},
        {5, typeof(ForgeDownloadRequest)}
    };

    public static Message GetMessageFor(MessageHeader header)
    {
        return (Message)Activator.CreateInstance(_messages[header.MessageKey]);
    }

    public static string GetMessageTypeName(MessageHeader header)
    {
        return _messages[header.MessageKey].Name;
    }

    public static byte GetKeyForMessageType(Type messageType)
    {
        string typeName = messageType.Name;
        foreach (KeyValuePair<byte, Type> message in _messages)
        {
            if (message.Value.Name != typeName) continue;
            return message.Key;
        }
        throw new ArgumentOutOfRangeException(nameof(messageType), messageType.Name + "is not a registered message");
    }
}