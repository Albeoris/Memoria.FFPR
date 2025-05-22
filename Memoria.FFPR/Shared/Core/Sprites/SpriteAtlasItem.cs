using System;
using System.Collections.Generic;
using System.IO;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.IL2CPP;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using UnityEngine.U2D;

namespace Memoria.FFPR.Core;

public sealed class SpriteAtlasItem
{
    public readonly HashSet<String> _usedPaths = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<String, (Int32 Index, Sprite Sprite)> _spritesByName = new();
    private readonly List<Sprite> _sprites;

    public IReadOnlyList<Sprite> Sprites => _sprites;

    public SpriteAtlasItem(List<Sprite> sprites)
    {
        _sprites = sprites ?? throw new ArgumentNullException(nameof(sprites));
        
        foreach (var sprite in Sprites)
            _spritesByName.Add(sprite.name, (_spritesByName.Count, sprite));
    }

    public static List<Sprite> GetSprites(SpriteAtlas atlas)
    {
        if (atlas is null)
            throw new ArgumentNullException(nameof(atlas));

        Int32 spriteCount = atlas.spriteCount;
        Il2CppReferenceArray<Sprite> array = new(spriteCount);
        if (atlas.GetSprites(array) != spriteCount)
            throw new ArgumentException(nameof(atlas));

        return array.ToManagedList();
    }

    public Boolean FindSprite(String name, out Sprite sprite)
    {
        if (_spritesByName.TryGetValue(name, out var pair))
        {
            sprite = pair.Sprite;
            return true;
        }

        sprite = null;
        return false;
    }

    public void Modify(SpriteSheetInfo sheet, Texture2D texture)
    {
        Int32 scale = CalcScale(sheet, texture);

        foreach (SpriteInfo info in sheet.Sprites)
        {
            Il2CppStructArray<Vector2> vertices = info.ScaleVertices(scale);
            Il2CppStructArray<UInt16> triangles = info.Triangles.ToStructArray();

            Sprite sprite = Sprite.Create(texture,
                info.TextureRect.Scale(scale),
                info.Pivot,
                pixelsPerUnit: 1 * scale,
                0,
                SpriteMeshType.Tight,
                ExtensionMethods.Scale(info.BordersRect, scale));
            sprite.name = info.Name;
            
            sprite.OverrideGeometry(vertices, triangles);

            if (_spritesByName.TryGetValue(sprite.name, out var pair))
            {
                _spritesByName[sprite.name] = (pair.Index, sprite);
                _sprites[pair.Index] = sprite;
            }
            else
            {
                _spritesByName.Add(sprite.name, (_sprites.Count, sprite));
                _sprites.Add(sprite);
            }
        }
    }

    private static Int32 CalcScale(SpriteSheetInfo sheet, Texture2D texture)
    {
        Int32 scaleX = 1;
        Int32 scaleY = 1;
        var aw = sheet.AtlasSize.Width;
        if (aw > 0)
        {
            Int32 tw = texture.width;
            if (tw < aw)
                throw new NotSupportedException($"The texture width {tw} cannot be smaller than the atlas width {aw}.");

            if (tw % aw != 0)
                throw new NotSupportedException($"The texture has the wrong width scale: {tw} (texture) / {aw} (atlas) = {(Single)tw / aw}. Only integer scaling is supported.");

            scaleX = tw / aw;
        }

        var ah = sheet.AtlasSize.Height;
        if (ah > 0)
        {
            Int32 th = texture.height;
            if (th < ah)
                throw new NotSupportedException($"The texture height {th} cannot be smaller than the atlas height {ah}.");

            if (th % ah != 0)
                throw new NotSupportedException($"The texture has the wrong height scale: {th} (texture) / {ah} (atlas) = {(Single)th / ah}. Only integer scaling is supported.");

            scaleY = th / ah;
        }

        if (scaleX != scaleY)
            throw new NotSupportedException($"Different scale of textures in width and height is not supported. Scale: ({scaleX}, {scaleY})");

        return scaleX;
    }

    public Boolean AddUsedPath(String tpsheetPath)
    {
        return _usedPaths.Add(Path.GetFullPath(tpsheetPath));
    }
}