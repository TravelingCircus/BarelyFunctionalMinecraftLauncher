using System.IO;
using CmlLib.Core;

namespace BFML.Core;

public static class Utils
{
    private static MinecraftPath _minecraftPath = new MinecraftPath();
    private static string _BFMLDirectory => _minecraftPath.BasePath + @"/BFML/";
    private static string _skinPath => _BFMLDirectory + "skin.png";

    public static void SaveSkin(string path)
    {
        if(File.Exists(_skinPath))File.Delete(_skinPath);
        File.Copy(path, _skinPath);
    }
}