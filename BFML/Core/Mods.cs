using System;
using CmlLib.Core;

namespace BFML.Core;

public class Mods
{
    private readonly MinecraftPath _minecraftPath;

    public Mods(MinecraftPath minecraftPath)
    {
        _minecraftPath = minecraftPath;
    }

    public bool ChecksumMatches(string checksum)
    {
        throw new NotImplementedException();
    }
}