using System;
using Version = Utils.Version;

namespace BFML.Core;

internal sealed class ModPack
{
    internal Guid Guid { get; private set; }
    internal string Name { get; private set; }
    internal Version ModPackVersion { get; private set; }
    internal Version VanillaVersion { get; private set; }
    internal Version ForgeVersion { get; private set; }
    internal string Changelog { get; private set; }
    internal ulong ModsChecksum { get; private set; }
    internal Mod[] Mods { get; private set; }
}
