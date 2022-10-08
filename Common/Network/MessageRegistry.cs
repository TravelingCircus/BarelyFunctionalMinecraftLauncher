﻿using CommonData.Network.Messages;
using CommonData.Network.Messages.LaunchConfiguration;
using CommonData.Network.Messages.Login;
using CommonData.Network.Messages.Registration;
using CommonData.Network.Messages.Skin;
using CommonData.Network.Messages.Version;

namespace CommonData.Network;

public static class MessageRegistry
{
    private static readonly Dictionary<byte, Type> _messages = new Dictionary<byte, Type>()
    {
        {1, typeof(RegistrationRequest)},
        {2, typeof(RegistrationResponse)},
        {3, typeof(LoginRequest)},
        {4, typeof(LoginResponse)},
        {5, typeof(LaunchConfigurationRequest)},
        {6, typeof(LaunchConfigurationResponse)},
        {7, typeof(ConfigVersionRequest)},
        {8, typeof(ConfigVersionResponse)},
        {9, typeof(SkinChangeRequest)},
        {10, typeof(SkinChangeResponse)},
        {11, typeof(ForgeDownloadRequest)},
        {12, typeof(ForgeDownloadResponse)}
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