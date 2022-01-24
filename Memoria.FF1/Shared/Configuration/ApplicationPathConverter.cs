using System;
using UnityEngine;

namespace Memoria.FFPR.Configuration;

public static class ApplicationPathConverter
{
    public static readonly String StreamingAssetsWindowsPath = WinPath(Application.streamingAssetsPath);
    public static readonly String DataWindowsPath = WinPath(Application.dataPath);
    public static readonly String PersistentDataWindowsPath = WinPath(Application.persistentDataPath);
    public static readonly String TemporaryCacheWindowsPath = WinPath(Application.temporaryCachePath);

    public static String ReplacePlaceholders(String path)
    {
        return path
            .Replace("%StreamingAssets%", StreamingAssetsWindowsPath)
            .Replace("%DataPath%", DataWindowsPath)
            .Replace("%PersistentDataPath%", PersistentDataWindowsPath)
            .Replace("%TemporaryCachePath%", TemporaryCacheWindowsPath);
    }

    public static String ReturnPlaceholders(String path)
    {
        return path
            .Replace(StreamingAssetsWindowsPath, "%StreamingAssets%")
            .Replace(DataWindowsPath, "%DataPath%")
            .Replace(PersistentDataWindowsPath, "%PersistentDataPath%")
            .Replace(TemporaryCacheWindowsPath, "%TemporaryCachePath%");
    }

    private static String WinPath(String linuxPath) => linuxPath.Replace('/', '\\');
}