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
    public int DedicatedRAM;
    public bool IsFullscreen;
    
    private static readonly string BFMLDirectoryPath = new MinecraftPath().BasePath + "\\BFML";
    private static readonly string PrefsPath = new MinecraftPath().BasePath + "\\BFML\\LocalPrefs.xml";

    public LocalPrefs()
    {
    }

    public LocalPrefs(string nickname, string password, int dedicatedRam = 3072, bool isFullscreen = false)
    {
        Nickname = nickname;
        Password = password;
        DedicatedRAM = dedicatedRam;
        IsFullscreen = isFullscreen;
    }

    public static void SaveLocalPrefs(string nickname, string password)
    {
        if (!Directory.Exists(BFMLDirectoryPath)) Directory.CreateDirectory(BFMLDirectoryPath);
        if (File.Exists(PrefsPath)) File.Delete(PrefsPath);
        using FileStream fileStream = new FileStream(PrefsPath, FileMode.OpenOrCreate);
        XmlSerializer xml = new XmlSerializer(typeof(LocalPrefs));
        xml.Serialize(fileStream, new LocalPrefs(nickname, password));
    }
    
    public static LocalPrefs GetLocalPrefs()
    {
        if (!File.Exists(PrefsPath))
        {
            SaveLocalPrefs("None", "None");
            return new LocalPrefs("None", "None");
        }
        using FileStream fileStream = new FileStream(PrefsPath, FileMode.Open);
        XmlSerializer serializer = new XmlSerializer(typeof(LocalPrefs));
        LocalPrefs prefs = (serializer.Deserialize(fileStream) as LocalPrefs)!;
        if (prefs is null) throw new InvalidDataException("Invalid local prefs xml stream");
        return prefs;
    }

    public static void Clear()
    {
        File.Delete(PrefsPath);
        SaveLocalPrefs("None", "None");
    }
}