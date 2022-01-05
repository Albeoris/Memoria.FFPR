using System;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Last.Management;
using Last.Map;
using Memoria.FFPR.Configuration;
using Memoria.FFPR.Core;
using UnhollowerBaseLib;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;
using Boolean = System.Boolean;
using Exception = System.Exception;
using File = System.IO.File;
using IntPtr = System.IntPtr;
using Object = Il2CppSystem.Object;
using Path = System.IO.Path;

namespace Memoria.FFPR.IL2CPP
{
    [HarmonyPatch(typeof(ContentCatalogData), nameof(ContentCatalogData.CreateLocator))]
    public sealed class ContentCatalogData_CreateLocator : Il2CppSystem.Object
    {
        private static readonly Dictionary<Object, Object> KnownCatalogs = new();
        
        public ContentCatalogData_CreateLocator(IntPtr ptr) : base(ptr)
        {
        }

        public static String GetFileExtension(String assetAddress)
        {
            return GetFileExtension("AddressablesMainContentCatalog", assetAddress);
        }
        
        public static String GetFileExtension(String catalogName, String assetAddress)
        {
            if (!KnownCatalogs.ContainsKey(catalogName))
                return String.Empty;

            Dictionary<Object, Object> catalog = KnownCatalogs[catalogName].Cast<Dictionary<Object, Object>>();
            if (!catalog.ContainsKey(assetAddress))
                return String.Empty;

            return catalog[assetAddress].ToString();
        }

        public static void Prefix(ContentCatalogData __instance)
        {
            String locatorId = __instance.location.PrimaryKey;
            if (KnownCatalogs.ContainsKey(locatorId))
                return;

            Dictionary<Object, Object> extensions = new();
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
    
    [HarmonyPatch(typeof(EncounterLot), nameof(EncounterLot.CheckEncount))]
    public sealed class EncounterLot_CheckEncount : Il2CppSystem.Object
    {
        public EncounterLot_CheckEncount(IntPtr ptr) : base(ptr)
        {
        }

        public static Boolean Prefix(ref Boolean __result)
        {
            if (ModComponent.Instance.EncountersControl.DisableEncounters)
            {
                __result = false;
                
                // Skip native method
                return false;
            }

            // Call native method
            return true;
        }
    }
}