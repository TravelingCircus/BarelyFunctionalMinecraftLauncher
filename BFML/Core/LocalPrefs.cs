using System;
using System.IO;
using System.Xml.Serialization;
using CmlLib.Core;

namespace BFML.Core;

[Serializable]
public sealed class LocalPrefs
{
    public string Nickname;
    public string Password;
    
    private static readonly string PrefsPath = new MinecraftPath().BasePath + "\\BFML\\LocalPrefs.xml";

    public LocalPrefs()
    {
    }

    public LocalPrefs(string nickname, string password)
    {
        Nickname = nickname;
        Password = password;
    }

    public static void SaveLocalPrefs(string nickname, string password)
    {
        using FileStream fileStream = new FileStream(PrefsPath, FileMode.OpenOrCreate);
        XmlSerializer xml = new XmlSerializer(typeof(LocalPrefs));
        xml.Serialize(fileStream, new LocalPrefs(nickname, password));
    }
    
    public static LocalPrefs GetLocalPrefs()
    {
        if (!File.Exists(PrefsPath)) throw new FileNotFoundException("Local prefs file doesn't exists");
        using FileStream fileStream = new FileStream(PrefsPath, FileMode.Open);
        XmlSerializer serializer = new XmlSerializer(typeof(LocalPrefs));
        LocalPrefs prefs = (serializer.Deserialize(fileStream) as LocalPrefs)!;
        if (prefs is null) throw new InvalidDataException("Invalid local prefs xml stream");
        return prefs;
        
    }
}