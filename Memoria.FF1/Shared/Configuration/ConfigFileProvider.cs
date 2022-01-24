using System;
using BepInEx;
using BepInEx.Configuration;
using Memoria.FFPR.Core;

namespace Memoria.FFPR.Configuration;

public sealed class ConfigFileProvider
{
    public ConfigFile Get(String sectionName)
    {
        String configPath = GetConfigurationPath(sectionName);
        return new ConfigFile(configPath, true, ownerMetadata: null);
    }
        
    private static String GetConfigurationPath(String sectionName)
    {
        return Utility.CombinePaths(Paths.ConfigPath, ModConstants.Id, sectionName + ".cfg");
    }
}