using System;
using Memoria.FF1.IL2CPP;
using HarmonyLib;
using Il2CppSystem.Asset;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.IO;
using Il2CppSystem.Reflection;
using Last.Management;
using Last.Map.Animation;
using Memoria.FF1.Configuration;
using UnhollowerBaseLib;
using UnityEngine;
using File = System.IO.File;
using MethodInfo = System.Reflection.MethodInfo;
using Object = Il2CppSystem.Object;
using Path = System.IO.Path;

namespace Memoria.FF1.IL2CPP
{
    [HarmonyPatch(typeof(ResourceManager), "IsLoadAssetCompleted")]
    public sealed class ResourceManager_IsLoadAssetCompleted : Il2CppSystem.Object
    {
        public ResourceManager_IsLoadAssetCompleted(IntPtr ptr) : base(ptr)
        {
        }

        // Don't use other Dictionaries. It must be present in IL2CPP
        private static readonly Dictionary<Object, Object> KnownAssets = new();

        public static void Postfix(String addressName, ResourceManager __instance, Boolean __result)
        {
            if (!__result)
                return;

            try
            {
                AssetsConfiguration config = ModComponent.Instance.Config.Assets;
                String importDirectory = config.ImportDirectory;
                String exportDirectory = config.ExportDirectory;

                if (importDirectory == String.Empty && exportDirectory == String.Empty)
                    return;

                const String DataPath = "Assets/GameAssets/Serial/Data/";
                if (!addressName.StartsWith(DataPath))
                    return;

                // Don't use TryGetValue to avoid MissingMethod exception
                IntPtr knownAsset = IntPtr.Zero;
                if (KnownAssets.ContainsKey(addressName))
                    knownAsset = KnownAssets[addressName].Pointer;

                // Skip if asset was already processed
                Object assetObject = ResourceManager.Instance.completeAssetDic[addressName];
                if (knownAsset == assetObject.Pointer)
                    return;
                
                KnownAssets[addressName] = assetObject;

                Boolean isJustExported = false;
                if (exportDirectory != String.Empty)
                {
                    String fullPath = Path.Combine(exportDirectory, addressName) + ".txt";
                    try
                    {
                        if (File.Exists(fullPath))
                        {
                            // ModComponent.Log.LogInfo($"[Export] File already exists: {fullPath}");
                        }
                        else
                        {
                            String directoryPath = Path.GetDirectoryName(fullPath);
                            Directory.CreateDirectory(directoryPath);
                            TextAsset asset = new TextAsset(assetObject.Pointer);
                            File.WriteAllText(fullPath, asset.text);
                            ModComponent.Log.LogInfo($"[Export] File exported: {fullPath}");
                            isJustExported = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ModComponent.Log.LogError($"[Export] Failed to export file [{fullPath}]: {ex}");
                    }
                }

                if (!isJustExported && importDirectory != String.Empty)
                {
                    String fullPath = Path.Combine(exportDirectory, addressName) + ".txt";
                    try
                    {
                        if (!File.Exists(fullPath))
                        {
                            // ModComponent.Log.LogInfo($"[Import] File does not exist: {fullPath}");
                        }
                        else
                        {
                            TextAsset newAsset = new TextAsset(File.ReadAllText(fullPath));

                            Dictionary<String, Object> dic = ResourceManager.Instance.completeAssetDic;
                            dic[addressName] = newAsset;

                            KnownAssets[addressName] = newAsset;
                            ModComponent.Log.LogInfo($"[Import] File imported: {fullPath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        ModComponent.Log.LogError($"[Import] Failed to import file [{fullPath}]: {ex}");
                    }
                }
            }
            catch (Exception ex)
            {
                ModComponent.Log.LogError($"[Fatal error]  {ex}");
            }
        }
    }
}