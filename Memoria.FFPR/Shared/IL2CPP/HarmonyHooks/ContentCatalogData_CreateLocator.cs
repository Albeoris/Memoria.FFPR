using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using Last.Data;
using Memoria.FFPR.IL2CPP;
using UnityEngine.AddressableAssets.ResourceLocators;
using Exception = System.Exception;
using IntPtr = System.IntPtr;
using Object = System.Object;
using String = System.String;

// ReSharper disable InconsistentNaming

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

[HarmonyPatch(typeof(ContentCatalogData), nameof(ContentCatalogData.CreateLocator))]
public static class ContentCatalogData_CreateLocator
{
    private static readonly Dictionary<String, Dictionary<String,String>> KnownCatalogs = new();
        
    public static String GetFileExtension(String assetAddress)
    {
        // ReSharper disable once StringLiteralTypo
        return GetFileExtension("AddressablesMainContentCatalog", assetAddress);
    }

    public static String GetFileExtension(String catalogName, String assetAddress)
    {
        return
            KnownCatalogs.TryGetValue(catalogName, out var dictionary)
            && dictionary.TryGetValue(assetAddress, out var extension)
                ? extension
                : String.Empty;
    }

    public static void Prefix(ContentCatalogData __instance)
    {
        String locatorId = __instance.location.PrimaryKey;
        if (KnownCatalogs.ContainsKey(locatorId))
            return;

        Dictionary<String, String> extensions = new();
        KnownCatalogs.Add(locatorId, extensions);

        try
        {
            foreach (String path in __instance.InternalIds)
            {
                String extension = Path.GetExtension(path);
                String pathWithoutExtension = path.Substring(0, path.Length - extension.Length);
                extensions[pathWithoutExtension] = extension;
            }
                
            ModComponent.Log.LogInfo($"[ContentCatalogData_CreateLocator] Resource file extensions have been successfully parsed.");
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogError($"[ContentCatalogData_CreateLocator] Failed to parse resource file extensions from catalog {__instance.location.InternalId}. Error: {ex}");
        }
    }
}