using Common.Network.Messages.ChangeSkin;
using Common.Network.Messages.ForgeDownload;
using Common.Network.Messages.GetSkin;
using Common.Network.Messages.Login;
using Common.Network.Messages.ModsDownload;
using Common.Network.Messages.Registration;

namespace Common.Network;

public static class MessageRegistry
{
    private static readonly Dictionary<byte, Type> _messages = new Dictionary<byte, Type>()
    {
        {1, typeof(RegistrationRequest)},
        {2, typeof(RegistrationResponse)},
        {3, typeof(LoginRequest)},
        {4, typeof(LoginResponse)},
        {9, typeof(SkinChangeRequest)},
        {10, typeof(SkinChangeResponse)},
        {11, typeof(ForgeDownloadRequest)},
        {12, typeof(ForgeDownloadResponse)},
        {13, typeof(ModsDownloadRequest)},
        {14, typeof(ModsDownloadResponse)},
        {15, typeof(GetSkinRequest)},
        {16, typeof(GetSkinResponse)}
    };

    public static Message GetMessageFor(MessageHeader header) => (Message)Activator.CreateInstance(GetMessageType(header));

    public static string GetMessageTypeName(MessageHeader header) => GetMessageType(header).Name;

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

    private static Type GetMessageType(MessageHeader header) => _messages[header.MessageKey];
}