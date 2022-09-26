﻿using System.Xml.Serialization;
using CommonData.Models;

namespace CommonData;

public static class DataSerializer
{
    public static void UserToXml(User user, Stream buffer)
    {
        XmlSerializer xml = new XmlSerializer(typeof(User));
        xml.Serialize(buffer, user);
    }
    
    public static User UserFromXml(Stream stream)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(User));
        User user = (serializer.Deserialize(stream) as User)!;
        if (user is null) throw new InvalidDataException("Invalid user xml stream");
        return user;
    }

    public static LaunchConfiguration LaunchConfigFromXml(Stream stream)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(LaunchConfiguration));
        LaunchConfiguration launchConfig = (serializer.Deserialize(stream) as LaunchConfiguration)!;
        if (launchConfig is null) throw new InvalidDataException("Invalid launchConfig xml stream");
        return launchConfig;
    }
    
    public static ConfigurationVersion ConfigVersionFromXml(Stream stream)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(ConfigurationVersion));
        ConfigurationVersion version = (serializer.Deserialize(stream) as ConfigurationVersion)!;
        if (version is null) throw new InvalidDataException("Invalid launchConfig xml stream");
        return version;
    }
}