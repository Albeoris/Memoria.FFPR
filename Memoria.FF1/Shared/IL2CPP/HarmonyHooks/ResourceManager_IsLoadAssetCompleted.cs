using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppSystem.IO;
using Last.Management;
using Memoria.FFPR.Configuration;
using Memoria.FFPR.Configuration.Scopes;
using Memoria.FFPR.Core;
using UnityEngine;
using UnityEngine.U2D;
using Boolean = System.Boolean;
using Exception = System.Exception;
using File = System.IO.File;
using IntPtr = System.IntPtr;
using Object = Il2CppSystem.Object;
using Path = System.IO.Path;
using String = System.String;

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

// TODO: Import before loading native resources. 
[HarmonyPatch(typeof(ResourceManager), nameof(ResourceManager.IsLoadAssetCompleted))]
public sealed class ResourceManager_IsLoadAssetCompleted : Il2CppSystem.Object
{
    public ResourceManager_IsLoadAssetCompleted(IntPtr ptr) : base(ptr)
    {
    }

    // Don't use other Dictionaries. It must be present in IL2CPP
    private static readonly Dictionary<String, Object> KnownAssets = new();
    private static readonly AssetExtensionResolver ExtensionResolver = new();

    public static void Postfix(String addressName, ResourceManager __instance, Boolean __result)
    {
        if (!__result)
            return;

        // Skip scenes or the game will crash
        if (addressName.StartsWith("Assets/Scenes"))
            return;

        try
        {
            AssetsConfiguration config = ModComponent.Instance.Config.Assets;
            String importDirectory = config.ImportDirectory;
            String exportDirectory = config.ExportDirectory;
            String modsDirectory = config.ModsDirectory;

            // Skip import if disabled
            if (importDirectory == String.Empty && modsDirectory == String.Empty)
                return;

            // Skip import if export is enabled to avoid race condition
            if (exportDirectory != String.Empty)
                return;

            IntPtr knownAsset = IntPtr.Zero;
            if (KnownAssets.TryGetValue(addressName, out var ka))
                knownAsset = ka.Pointer;

            Il2CppSystem.Collections.Generic.Dictionary<String, Object> dic = ResourceManager.Instance.completeAssetDic;
            if (!dic.ContainsKey(addressName))
                return;

            Object assetObject = dic[addressName];
            if (assetObject is null)
                return;

            // Skip if asset was already processed
            if (knownAsset == assetObject.Pointer)
                return;

            String type = ExtensionResolver.GetAssetType(assetObject);
            String extension = ExtensionResolver.GetFileExtension(addressName);
            if (type == "UnityEngine.Sprite")
                extension = ".png";
            String addressNameWithExtension = addressName + extension;

            String fullPath = Path.Combine(importDirectory, addressNameWithExtension);
            if (File.Exists(fullPath) || Directory.Exists(fullPath))
            {
                if (TryImportAsset(type, config, assetObject, fullPath, out var newAsset))
                {
                    assetObject = newAsset;
                    String shortPath = ApplicationPathConverter.ReturnPlaceholders(fullPath);
                    ModComponent.Log.LogInfo($"[Import] File imported: {shortPath}");
                }
            }
            else if (File.Exists(fullPath + ".ini"))
            {
                fullPath += ".ini";
                if (TryImportAssetReference(type, config, assetObject, fullPath, out var newAsset))
                {
                    assetObject = newAsset;
                    String shortPath = ApplicationPathConverter.ReturnPlaceholders(fullPath);
                    ModComponent.Log.LogInfo($"[Import] File imported: {shortPath}");
                }
            }

            IReadOnlyList<String> modPath = ModComponent.Instance.ModFiles.FindAll(addressNameWithExtension);
            if (modPath.Count > 0)
            {
                if (TryModAsset(type, config, assetObject, modPath, out var newAsset))
                    assetObject = newAsset;
            }

            IReadOnlyList<String> modReferences = ModComponent.Instance.ModFiles.FindAll(addressNameWithExtension + ".ini");
            if (modReferences.Count > 0)
            {
                if (TryModAssetByReference(type, config, assetObject, modReferences, out var newAsset))
                    assetObject = newAsset;
            }

            dic[addressName] = assetObject;
            KnownAssets[addressName] = assetObject;
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogError($"[Import] Failed to import file [{addressName}]: {ex}");
        }
    }

    private static Boolean TryImportAssetReference(String type, AssetsConfiguration config, Object assetObject, String fullPath, out Object newAsset)
    {
        newAsset = null;
        switch (type)
        {
            case "UnityEngine.Sprite":
            {
                if (!config.ImportTextures)
                    return false;
                return TryImportSpriteByReference(fullPath, out newAsset);
            }
        }

        return false;
    }

    private static Boolean TryImportSpriteByReference(String fullPath, out Object result)
    {
        // TODO: Ini Reader
        String[] lines = File.ReadAllLines(fullPath);

        String AtlasPath = null;
        String SpriteName = null;
        String AtlasGroup = null;
        String AtlasName = null;
        foreach (String line in lines)
        {
            Int32 index = line.IndexOf('=');
            if (index < 0)
                continue;

            String key = line.Substring(0, index).Trim();
            String value = line.Substring(index + 1).Trim();
            switch (key)
            {
                case "AtlasPath":
                    AtlasPath = value;
                    break;
                case "SpriteName":
                    SpriteName = value;
                    break;
                case "AtlasGroup":
                    AtlasGroup = value;
                    break;
                case "AtlasName":
                    AtlasName = value;
                    break;
            }
        }

        String shortReferencePath = ApplicationPathConverter.ReturnPlaceholders(fullPath);
        String directory = Path.GetDirectoryName(fullPath);
        String atlasPath = Path.Combine(directory, AtlasPath);
        String atlasFullPath = Path.GetFullPath(atlasPath);
        // String atlasName = Path.GetFileNameWithoutExtension(atlasFullPath);
        // String tpsheetPath = Path.Combine(atlasFullPath, $"{atlasName}.tpsheet");
        
        if (SpriteAtlasCache.FindAtlas(AtlasName, out var atlas))
        {
        }
        else if (Directory.Exists(atlasFullPath))
        {
            ResourceManager resourceManager = ResourceManager.Instance;
            if (!resourceManager.CheckLoadAssetCompleted(AtlasGroup, AtlasName))
            {
                ModComponent.Log.LogWarning($"[Import] Cannot import sprite by reference to not loaded atlas [Group: {AtlasGroup}, Name: {AtlasName}], referenced by [{shortReferencePath}].");
                result = null;
                return false;
            }
            
            Il2CppSystem.Collections.Generic.Dictionary<String, Il2CppSystem.Object> loaded = resourceManager.completeAssetDic;
            SpriteAtlas sa = loaded[AtlasName].Cast<SpriteAtlas>();;
            if (sa is null)
            {
                ModComponent.Log.LogWarning($"[Import] Cannot import sprite by reference to unloaded atlas [Group: {AtlasGroup}, Name: {AtlasName}], referenced by [{shortReferencePath}].");
                result = null;
                return false;
            }
            atlas = SpriteAtlasCache.Import(sa, atlasFullPath);

            String extension = ExtensionResolver.GetFileExtension(AtlasName);
            String addressNameWithExtension = AtlasName + extension;
            IReadOnlyList<String> modPath = ModComponent.Instance.ModFiles.FindAll(addressNameWithExtension);
            SpriteAtlasCache.Modify(sa, modPath);
        }
        else
        {
            String shortAtlasPath = ApplicationPathConverter.ReturnPlaceholders(atlasFullPath);
            ModComponent.Log.LogWarning($"[Import] Cannot find atlas [{shortAtlasPath}], referenced by [{shortReferencePath}].");
            result = null;
            return false;
        }

        if (atlas.FindSprite(SpriteName, out var sprite))
        {
            result = sprite;
            return true;
        }
        
        ModComponent.Log.LogWarning($"[Import] Cannot find sprite [{SpriteName}], referenced by [{shortReferencePath}].");
        result = null;
        return false;
    }

    private static Boolean TryImportAsset(String type, AssetsConfiguration config, Object assetObject, String fullPath, out Object newAsset)
    {
        newAsset = null;
        switch (type)
        {
            case "UnityEngine.AnimationClip":
            case "UnityEngine.AnimatorOverrideController":
            case "UnityEngine.GameObject":
            case "UnityEngine.Material":
            case "UnityEngine.RenderTexture":
            case "UnityEngine.RuntimeAnimatorController":
            case "UnityEngine.Shader":
            default:
                throw new NotSupportedException(type);
            case "UnityEngine.TextAsset":
            {
                if (!config.ImportText)
                    return false;
                newAsset = ImportTextAsset(fullPath);
                break;
            }
            case "System.Byte[]":
            {
                if (!config.ImportBinary)
                    return false;
                newAsset = ImportBinaryAsset(assetObject.Cast<TextAsset>().name, fullPath);
                break;
            }
            case "UnityEngine.Texture2D":
            {
                if (!config.ImportTextures)
                    return false;
                newAsset = ImportTextures(assetObject.Cast<Texture2D>().filterMode, fullPath);
                break;
            }
            case "UnityEngine.Sprite":
            {
                if (!config.ImportTextures)
                    return false;
                newAsset = ImportSprite(assetObject.Cast<Sprite>(), fullPath);
                break;
            }
            case "UnityEngine.U2D.SpriteAtlas":
            {
                if (!config.ImportTextures)
                    return false;
                newAsset = ImportSpriteAtlas(assetObject.Cast<SpriteAtlas>(), fullPath);
                break;
            }
        }

        return true;
    }

    private static Object ImportTextAsset(String fullPath)
    {
        return new TextAsset(File.ReadAllText(fullPath));
    }

    private static Object ImportTextures(FilterMode filterMode, String fullPath)
    {
        return TextureHelper.ReadTextureFromFile(filterMode, fullPath);
    }

    private static Object ImportSpriteAtlas(SpriteAtlas asset, String fullPath)
    {
        SpriteAtlasCache.Import(asset, fullPath);
        return asset;
    }

    private static Object ImportSprite(Sprite asset, String fullPath)
    {
        Rect originalRect = asset.rect;
        Vector2 originalPivot = asset.pivot;
        Single originalWidth = asset.texture.width;
        Single originalHeight = asset.texture.height;

        Texture2D texture = TextureHelper.ReadTextureFromFile(asset.texture.filterMode, fullPath);

        texture.wrapMode = asset.texture.wrapMode;
        texture.wrapModeU = asset.texture.wrapModeU;
        texture.wrapModeV = asset.texture.wrapModeV;
        texture.wrapModeW = asset.texture.wrapModeW;

        Single newWidth = texture.width;
        Single newHeight = texture.height;
        Single ox = newWidth / originalWidth;
        Single oy = newHeight / originalHeight;
        Single px = originalPivot.x / originalWidth;
        Single py = originalPivot.y / originalHeight;
        Rect newRect = new Rect(originalRect.x * ox, originalRect.y * oy, originalRect.width * ox, originalRect.height * oy);

        Vector2 newPivot = new Vector2(px, py);
        Sprite newSpr = Sprite.Create(texture, newRect, newPivot, asset.pixelsPerUnit, 0, SpriteMeshType.Tight, asset.border);
        newSpr.name = asset.name;
        return newSpr;
    }

    private static Object ImportBinaryAsset(String assetName, String fullPath)
    {
        // Il2CppStructArray<Byte> sourceBytes = Il2CppSystem.IO.File.ReadAllBytes(fullPath);

        // Not working
        // TextAsset result = new TextAsset(new String('a', sourceBytes.Length));
        // result.name = assetName + ".bytes";
        // Il2CppStructArray<Byte> targetBytes = result.bytes;
        // for (int i = 0; i < sourceBytes.Length; i++)
        //     targetBytes[i] = sourceBytes[i];

        // Not working
        // Char[] chars = new Char[sourceBytes.Length];
        // for (Int32 i = 0; i < sourceBytes.Length; i++)
        //     chars[i] = (Char)sourceBytes[i];
        //
        // TextAsset result = new TextAsset(new String(chars, 0, chars.Length)) { name = assetName + ".bytes" };
        //
        // return result;

        throw new NotSupportedException();
    }

    private static Boolean TryModAsset(String type, AssetsConfiguration config, Object assetObject, IReadOnlyList<String> modPath, out Object newAsset)
    {
        newAsset = null;
        switch (type)
        {
            case "UnityEngine.AnimationClip":
            case "UnityEngine.AnimatorOverrideController":
            case "UnityEngine.GameObject":
            case "UnityEngine.Material":
            case "UnityEngine.RenderTexture":
            case "UnityEngine.RuntimeAnimatorController":
            case "UnityEngine.Shader":
            {
                throw new NotSupportedException(type);
            }
            case "UnityEngine.Sprite":
            {
                String fullPath = modPath.Last();
                newAsset = ImportSprite(assetObject.Cast<Sprite>(), fullPath);

                String shortPath = ApplicationPathConverter.ReturnPlaceholders(fullPath);
                ModComponent.Log.LogInfo($"[Mod] Sprite replaced: {shortPath}");

                break;
            }
            case "UnityEngine.TextAsset":
            {
                String fullPath = modPath.Last();
                if (Path.GetExtension(fullPath) == ".csv")
                {
                    TextAsset textAsset = assetObject.Cast<TextAsset>();
                        
                    CsvMerger merger = new(textAsset.text);
                    merger.MergeFiles(modPath);

                    newAsset = new TextAsset(merger.BuildContent());
                }
                else
                {
                    newAsset = ImportTextAsset(fullPath);
                    String shortPath = ApplicationPathConverter.ReturnPlaceholders(fullPath);
                    ModComponent.Log.LogInfo($"[Mod] Text replaced: {shortPath}");
                }
                break;
            }
            case "System.Byte[]":
            {
                return false;
            }
            case "UnityEngine.Texture2D":
            {
                String fullPath = modPath.Last();
                Texture2D texture = assetObject.Cast<Texture2D>();
                newAsset = ImportTextures(texture.filterMode, fullPath);

                String shortPath = ApplicationPathConverter.ReturnPlaceholders(fullPath);
                ModComponent.Log.LogInfo($"[Mod] Texture replaced: {shortPath}");
                break;
            }
            case "UnityEngine.U2D.SpriteAtlas":
            {
                SpriteAtlas spriteAtlas = assetObject.Cast<SpriteAtlas>();

                SpriteAtlasCache.Modify(spriteAtlas, modPath);

                newAsset = spriteAtlas;
                break;
            }
        }

        return true;
    }
    
    private static Boolean TryModAssetByReference(String type, AssetsConfiguration config, Object assetObject, IReadOnlyList<String> modPath, out Object newAsset)
    {
        newAsset = null;
        switch (type)
        {
            case "UnityEngine.Sprite":
            {
                String fullPath = modPath.Last();
                if (TryImportSpriteByReference(fullPath, out newAsset))
                {
                    String shortPath = ApplicationPathConverter.ReturnPlaceholders(fullPath);
                    ModComponent.Log.LogInfo($"[Mod] Sprite replaced: {shortPath}");
                }

                break;
            }
        }
        return false;
    }
}