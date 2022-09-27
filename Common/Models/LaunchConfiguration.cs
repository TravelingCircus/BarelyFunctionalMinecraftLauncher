﻿namespace CommonData.Models;

[Serializable]
public sealed class LaunchConfiguration
{
    //TODO rewrite messages with new field
    
    public string VanillaVersion;
    public string ForgeVersion;
    public string ModsChecksum;
    public int RequiredGriwnas;

    public LaunchConfiguration()
    {
        
    }

    public LaunchConfiguration(string vanillaVersion, string forgeVersion, string modsChecksum, int requiredGriwnas)
    {
        VanillaVersion = vanillaVersion;
        ForgeVersion = forgeVersion;
        ModsChecksum = modsChecksum;
        RequiredGriwnas = requiredGriwnas;
    }
}