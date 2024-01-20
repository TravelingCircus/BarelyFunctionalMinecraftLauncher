using System;
using System.IO;
using BFML._3D;

namespace Common;

public sealed class Skin
{
    public readonly Texture Texture;

    public Skin(Texture texture)
    {
        Texture = texture;
    }
}