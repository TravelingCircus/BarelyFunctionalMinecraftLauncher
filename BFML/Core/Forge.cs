using System;
using CmlLib.Core;

namespace BFML.Core;

public class Forge
{
    private readonly MinecraftPath _minecraftPath;

    public Forge(MinecraftPath minecraftPath)
    {
        _minecraftPath = minecraftPath;
    }

    public bool IsInstalled()
    {
        throw new NotImplementedException();
    }
}