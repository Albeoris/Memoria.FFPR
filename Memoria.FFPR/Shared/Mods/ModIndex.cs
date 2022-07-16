using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Memoria.FFPR.Configuration;
using Memoria.FFPR.IL2CPP;
using UnhollowerBaseLib;
using UnityEngine;
using Object = Il2CppSystem.Object;

namespace Memoria.FFPR.Mods;

public sealed class ModFileResolver
{
    private readonly String _modsRoot;
    private readonly Dictionary<String, List<String>> _catalog;

    public ModFileResolver()
    {
        _modsRoot = ModComponent.Instance.Config.Assets.ModsDirectory;
        _catalog = IndexMods(_modsRoot);
    }

    public IReadOnlyList<String> FindAll(String assetAddress)
    {
        if (!_catalog.TryGetValue(assetAddress, out List<String> modNames))
            return Array.Empty<String>();

        return modNames.Select(n => Path.Combine(_modsRoot, n, assetAddress)).ToArray();
    }

    private static Dictionary<String, List<String>> IndexMods(String modsRoot)
    {
        Dictionary<String, List<String>> catalog = new(StringComparer.InvariantCultureIgnoreCase);

        if (!Directory.Exists(modsRoot))
        {
            ModComponent.Log.LogInfo($"[Mods] Mods indexing skipped. Mods directory is not defined.");
            return catalog;
        }

        String[] mods = Directory.GetDirectories(modsRoot);
        foreach (String modDirectory in mods)
        {
            String modName = Path.GetFileName(modDirectory);

            String shortPath = ApplicationPathConverter.ReturnPlaceholders(modDirectory);
            ModComponent.Log.LogInfo($"[Mods.{modName}] Indexing mod. Directory: {shortPath}.");
            String[] files = Directory.GetFiles(modDirectory, "*", SearchOption.AllDirectories);
            foreach (String file in files)
            {
                // Before: C:\Mods\My\Assets\GameAssets\File.txt
                // After:             Assets\GameAssets\File.txt
                String assetAddress = file.Substring(modDirectory.Length + 1);
                
                // Before: Assets\GameAssets\File.txt
                // After : Assets/GameAssets/File.txt
                assetAddress = assetAddress.Replace("\\", "/");
                if (!catalog.TryGetValue(assetAddress, out var modNames))
                {
                    modNames = new List<String>();
                    catalog.Add(assetAddress, modNames);
                }

                modNames.Add(modName);
                ModComponent.Log.LogInfo($"[Mods.{modName}] {assetAddress}.");
            }
        }

        return catalog;
    }
}