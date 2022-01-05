using System;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Last.Management;
using Last.Map;
using Memoria.FFPR.Configuration;
using Memoria.FFPR.Core;
using UnhollowerBaseLib;
using UnityEngine;
using Boolean = System.Boolean;
using Exception = System.Exception;
using File = System.IO.File;
using IntPtr = System.IntPtr;
using Object = Il2CppSystem.Object;
using Path = System.IO.Path;
using String = System.String;

namespace Memoria.FFPR.IL2CPP
{
    // TODO: Import before loading native resources. 
    [HarmonyPatch(typeof(ResourceManager), nameof(ResourceManager.IsLoadAssetCompleted))]
    public sealed class ResourceManager_IsLoadAssetCompleted : Il2CppSystem.Object
    {
        public ResourceManager_IsLoadAssetCompleted(IntPtr ptr) : base(ptr)
        {
        }

        // Don't use other Dictionaries. It must be present in IL2CPP
        private static readonly Dictionary<Object, Object> KnownAssets = new();
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

                // Skip import if disabled
                if (importDirectory == String.Empty)
                    return;

                // Skip import if export is enabled to avoid race condition
                if (exportDirectory != String.Empty)
                    return;

                // Don't use TryGetValue to avoid MissingMethod exception
                IntPtr knownAsset = IntPtr.Zero;
                if (KnownAssets.ContainsKey(addressName))
                    knownAsset = KnownAssets[addressName].Pointer;

                Dictionary<String, Object> dic = ResourceManager.Instance.completeAssetDic;
                if (!dic.ContainsKey(addressName))
                    return;
                
                Object assetObject = dic[addressName];
                if (assetObject is null)
                    return;

                // Skip if asset was already processed
                if (knownAsset == assetObject.Pointer)
                    return;
                
                KnownAssets[addressName] = assetObject;

                String type = ExtensionResolver.GetAssetType(assetObject);
                String extension = ExtensionResolver.GetFileExtension(addressName);
                String fullPath = Path.Combine(importDirectory, addressName) + extension;
                if (!File.Exists(fullPath))
                    return;

                Object newAsset = null;
                switch (type)
                {
                    case "UnityEngine.AnimationClip":
                    case "UnityEngine.AnimatorOverrideController":
                    case "UnityEngine.GameObject":
                    case "UnityEngine.Material":
                    case "UnityEngine.RenderTexture":
                    case "UnityEngine.RuntimeAnimatorController":
                    case "UnityEngine.Shader":
                    case "UnityEngine.Sprite":
                    {
                        if (!config.ImportTextures)
                            return;
                        newAsset = ImportSprite(assetObject.Cast<Sprite>(), fullPath);
                        break;
                    }
                    case "UnityEngine.TextAsset":
                    {
                        if (!config.ImportText)
                            return;
                        newAsset = ImportTextAsset(fullPath);
                        break;
                    }
                    case "System.Byte[]":
                    {
                        if (!config.ImportBinary)
                            return;
                        newAsset = ImportBinaryAsset(assetObject.Cast<TextAsset>().name, fullPath);
                        break;
                    }
                    case "UnityEngine.Texture2D":
                        if (!config.ImportTextures)
                            return;
                        newAsset = ImportTextures(fullPath);
                        break;
                    case "UnityEngine.U2D.SpriteAtlas":
                        throw new NotSupportedException(type);
                }

                dic[addressName] = newAsset;

                KnownAssets[addressName] = newAsset;

                String shortPath = ApplicationPathConverter.ReturnPlaceholders(fullPath);
                ModComponent.Log.LogInfo($"[Import] File imported: {shortPath}");
            }
            catch (Exception ex)
            {
                ModComponent.Log.LogError($"[Import] Failed to import file [{addressName}]: {ex}");
            }
        }

        private static Object ImportTextAsset(String fullPath)
        {
            return new TextAsset(File.ReadAllText(fullPath));
        }
        
        private static Object ImportTextures(String fullPath)
        {
            return TextureHelper.ReadTextureFromFile(fullPath);
        }
        
        private static Object ImportSprite(Sprite asset, String fullPath)
        {
            Rect originalRect = asset.rect;
            Vector2 originalPivot = asset.pivot;
            Single originalWidth = asset.texture.width;
            Single originalHeight = asset.texture.height;

            Texture2D texture = TextureHelper.ReadTextureFromFile(fullPath);
            Single newWidth = texture.width;
            Single newHeight = texture.height;
            Single ox = newWidth / originalWidth;
            Single oy = newHeight / originalHeight;
            Rect newRect = new Rect(originalRect.x * ox, originalRect.y * oy, originalRect.width * ox, originalRect.height * oy);
            Vector2 newPivot = new Vector2(originalPivot.x * ox, originalPivot.y * oy);

            return Sprite.Create(texture, newRect, newPivot, asset.pixelsPerUnit);
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
    }
}