using System;
using BepInEx.Configuration;

namespace Memoria.FFPR.Configuration;

public static class ExtensionsConfigFile
{
    public static ConfigEntry<T> Bind<T>(this ConfigFile file, String section, String key, T defaultValue, String description, params String[] tags) where T : IComparable
    {
        return file.Bind(section, key, defaultValue,
            new ConfigDescription(description, null, tags));
    }

    public static ConfigEntry<T> Bind<T>(this ConfigFile file, String section, String key, T defaultValue, String description, T minValue, T maxValue, params String[] tags) where T : IComparable
    {
        return file.Bind(section, key, defaultValue,
            new ConfigDescription(description, new AcceptableValueRange<T>(minValue, maxValue), tags));
    }

    public static ConfigEntry<T> Bind<T>(this ConfigFile file, String section, String key, T defaultValue, String description, AcceptableValueList<T> acceptable, params String[] tags) where T : IEquatable<T>
    {
        return file.Bind(section, key, defaultValue,
            new ConfigDescription(description, acceptable, tags));
    }
        
    public static ConfigEntry<T> Bind<T>(this ConfigFile file, String section, String key, T defaultValue, String description, AcceptableValueBase acceptable, params String[] tags) where T : IEquatable<T>
    {
        return file.Bind(section, key, defaultValue,
            new ConfigDescription(description, acceptable, tags));
    }
}