﻿namespace Common.Models;

public sealed class ModPackManifest
{
    public string Name;
    public string Version;
    public string Changelog;
    public string ForgeVersion;
    public string VanillaVersion;
    public ulong ModsChecksum;
}