using System;
using BepInEx.Configuration;

namespace Memoria.FF1.Configuration
{
    public static class ExtensionsConfigFile
    {
        public static ConfigEntry<T> Bind<T>(this ConfigFile file, String section, String key, T defaultValue, String description, params Object[] tags) where T : IComparable
        {
            return file.Bind(section, key, defaultValue,
                new ConfigDescription(description, null, tags));
        }

        public static ConfigEntry<T> Bind<T>(this ConfigFile file, String section, String key, T defaultValue, String description, T minValue, T maxValue, params Object[] tags) where T : IComparable
        {
            return file.Bind(section, key, defaultValue,
                new ConfigDescription(description, new AcceptableValueRange<T>(minValue, maxValue), tags));
        }

        public static ConfigEntry<T> Bind<T>(this ConfigFile file, String section, String key, T defaultValue, String description, AcceptableValueList<T> acceptable, params Object[] tags) where T : IEquatable<T>
        {
            return file.Bind(section, key, defaultValue,
                new ConfigDescription(description, acceptable, tags));
        }
    }
}