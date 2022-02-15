using System;
using System.Collections.Generic;
using System.IO;
using Memoria.FFPR.Configuration;
using Memoria.FFPR.IL2CPP;
using UnhollowerBaseLib;
using UnityEngine;
using UnityEngine.U2D;
using Object = System.Object;

namespace Memoria.FFPR.Core;

public sealed class SpriteAtlasCache
{
    private static readonly Dictionary<String, SpriteAtlasItem> _items = new();
    
    public static Boolean IsDisabled { get; set; }

    public static Boolean FindAtlas(String name, out SpriteAtlasItem item)
    {
        if (IsDisabled)
        {
            item = null;
            return false;
        }

        return _items.TryGetValue(name, out item);
    }

    public static SpriteAtlasItem Import(SpriteAtlas asset, String fullPath)
    {
        if (_items.TryGetValue(asset.name, out var result))
            return result;

        return Modify(asset, fullPath);
    }
    
    public static SpriteAtlasItem Modify(SpriteAtlas asset, String fullPath)
    {
        if (!_items.TryGetValue(asset.name, out var item))
        {
            item = Create(asset);
            _items.Add(asset.name, item);
        }

        String atlasName = Path.GetFileNameWithoutExtension(fullPath);
        String tpsheetPath = Path.Combine(fullPath, $"{atlasName}.tpsheet");
        if (!item.AddUsedPath(tpsheetPath))
            return item;
        
        SpriteSheetInfo spriteSheet = SpriteSheetInfo.ReadFromFile(tpsheetPath);

        FilterMode filterMode = GetFilterMode(asset);
        Texture2D texture = TextureHelper.ReadTextureFromFile(filterMode, spriteSheet.TexturePath);

        item.Modify(spriteSheet, texture);
        return item;
    }
    
    public static void Modify(SpriteAtlas spriteAtlas, IReadOnlyList<String> filePaths)
    {
        if (spriteAtlas is null) throw new ArgumentNullException(nameof(spriteAtlas));
        if (filePaths is null) throw new ArgumentNullException(nameof(filePaths));

        foreach (String fullPath in filePaths)
        {
            try
            {
                String shortPath = ApplicationPathConverter.ReturnPlaceholders(fullPath);
                ModComponent.Log.LogInfo($"[Mod] Merging data from {shortPath}");

                Modify(spriteAtlas, fullPath);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to merge data from {fullPath}", nameof(fullPath), ex);
            }
        }
    }

    private static SpriteAtlasItem Create(SpriteAtlas asset)
    {
        SpriteAtlas atlas = UnityEngine.Object.Instantiate(asset);
        List<Sprite> sprites = SpriteAtlasItem.GetSprites(atlas);
        return new SpriteAtlasItem(sprites);
    }
    
    private static FilterMode GetFilterMode(SpriteAtlas asset)
    {
        FilterMode filterMode = FilterMode.Point;
        if (asset.spriteCount < 1)
            return filterMode;
        
        Il2CppReferenceArray<Sprite> spriteArray = new(asset.spriteCount);
        asset.GetSprites(spriteArray);
        filterMode = spriteArray[0].texture.filterMode;

        return filterMode;
    }
}