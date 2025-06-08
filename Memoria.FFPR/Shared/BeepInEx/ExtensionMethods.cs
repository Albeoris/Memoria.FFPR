using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using BepInEx.Logging;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Memoria.FFPR.Configuration;
using Memoria.FFPR.IL2CPP;
using UnityEngine;

namespace Memoria.FFPR.BeepInEx;

public static class ExtensionMethods
{
    public static IReadOnlyList<T> DistinctBy<T, TKey>(this IEnumerable<T> self, Func<T, TKey> selector)
    {
        List<T> result;
        if (self is IReadOnlyCollection<T> collection)
            result = new List<T>(collection.Count);
        else
            result = new();

        HashSet<TKey> set = new();
        foreach (var item in self)
        {
            TKey key = selector(item);
            if (set.Add(key))
                result.Add(item);
        }

        return result;
    }

    public static void LogException(this ManualLogSource logSource, Exception ex)
    {
        logSource.LogError(ex.ToString());
    }
    
    public static void LogException(this ManualLogSource logSource, Exception ex, String error)
    {
        logSource.LogError(error);
        logSource.LogError(ex.ToString());
    }
    
    public static void Deconstruct<TKey, TValue>(this Il2CppSystem.Collections.Generic.KeyValuePair<TKey, TValue> il2cpp, out TKey key, out TValue value)
    {
        key = il2cpp.Key;
        value = il2cpp.Value;
    }

    public static Rect Scale(this Rect rect, Vector2 scale)
    {
        return scale == Vector2.one
            ? rect
            : new Rect(rect.x * scale.x, rect.y * scale.y, rect.width * scale.x, rect.height * scale.y);
    }
    
    public static Rect Scale(this Rect rect, Int32 scale)
    {
        return scale == 1
            ? rect
            : new Rect(rect.x * scale, rect.y * scale, rect.width * scale, rect.height * scale);
    }

    public static Vector4 Scale(this Vector4 rect, Vector2 scale)
    {
        return scale == Vector2.one
            ? rect
            : new Vector4(
                x: rect.x * scale.x,
                y: rect.y * scale.y,
                z: rect.z * scale.x,
                w: rect.w * scale.y);
    }
    
    public static Vector4 Scale(this Vector4 rect, Int32 scale)
    {
        return scale == 1
            ? rect
            : new Vector4(
                x: rect.x * scale,
                y: rect.y * scale,
                z: rect.z * scale,
                w: rect.w * scale);
    }
    
    public static RectInt GetPixelSpriteRect(this Sprite sprite)
    {
        if (sprite is null) throw new ArgumentNullException(nameof(sprite));
        
        Rect rect = sprite.rect;
        RectInt result = new RectInt
        {
            x = (Int32)Math.Round(rect.x),
            y = (Int32)Math.Round(rect.y),
            width = (Int32)Math.Round(rect.width),
            height = (Int32)Math.Round(rect.height)
        };

        return result;
    }
    
    public static RectInt GetPixelTextureRect(this Sprite sprite)
    {
        if (sprite is null) throw new ArgumentNullException(nameof(sprite));
        
        Rect rect = sprite.GetTextureRect();
        RectInt result = new RectInt
        {
            x = (Int32)Math.Round(rect.x),
            y = (Int32)Math.Round(rect.y),
            width = (Int32)Math.Round(rect.width),
            height = (Int32)Math.Round(rect.height)
        };

        return result;
    }

    public static Vector2Int GetPixelPivot(this Sprite sprite)
    {
        if (sprite is null) throw new ArgumentNullException(nameof(sprite));
        
        Vector2 pivot = sprite.pivot;
        return new Vector2Int((Int32)pivot.x, (Int32)pivot.y);
    }
    
    public static Vector2 GetMeshPivot(this Sprite sprite)
    {
        if (sprite is null) throw new ArgumentNullException(nameof(sprite));

        Rect textureRect = sprite.GetTextureRect();
        if (textureRect.width == 0 || textureRect.height == 0)
            return Vector2.zero;
        
        Rect spriteRect = sprite.rect;
        if (spriteRect.width == 0 || spriteRect.height == 0)
            return Vector2.zero;
        
        Vector2 pivot = sprite.pivot;
        return new Vector2(pivot.x / textureRect.width, pivot.y / textureRect.height);
    }

    public static Vector2Int[] GetPixelVertices(this Sprite sprite)
    {
        if (sprite is null) throw new ArgumentNullException(nameof(sprite));

        Rect spriteRect = sprite.rect;
        Rect textureRect = sprite.GetTextureRect();
        RectInt pixelTextureRect = sprite.GetPixelTextureRect();
        Single scaleX = textureRect.width / spriteRect.width;
        Single scaleY = textureRect.height / spriteRect.height;
        Vector2 pixelPivot = sprite.pivot;
        Vector2[] meshVertices = sprite.GetMeshVertices();
        Vector2Int[] pixelVertices = new Vector2Int[meshVertices.Length];

        for (int i = 0; i < meshVertices.Length; i++)
        {
            Vector2 mesh = meshVertices[i];
            Single floatX = (mesh.x + pixelPivot.x) * scaleX - 0.01f;
            Single floatY = (mesh.y + pixelPivot.y) * scaleY - 0.01f;
            Vector2Int pixel = new Vector2Int((Int32)Math.Round(floatX), (Int32)Math.Round(floatY));

            if (pixel.x < 0 || pixel.y < 0 || pixel.x > pixelTextureRect.width || pixel.y > pixelTextureRect.height)
                throw new NotSupportedException($"During transformation the coordinate of the {i} vertex of the sprite, the coordinate went beyond the boundaries of the sprite. SpriteRect: {spriteRect}, TextureRect: {textureRect}, Pivot: {pixelPivot} TransformedVertex: ({floatX}, {floatY}), BeforeTransform: {mesh}");

            pixelVertices[i] = pixel;
        }

        return pixelVertices;
    }

    public static Vector2[] GetMeshVertices(this Sprite sprite)
    {
        if (sprite is null) throw new ArgumentNullException(nameof(sprite));

        return sprite.vertices.ToManaged();
    }
    

    public static T[] ToManaged<T>(this Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<T> il2cpp) where T : Il2CppObjectBase
    {
        if (il2cpp is null) throw new ArgumentNullException(nameof(il2cpp));
        
        T[] result = new T[il2cpp.Count];
        for (int i = 0; i < il2cpp.Length; i++)
            result[i] = il2cpp[i];
        return result;
    }
    
    public static T[] ToManaged<T>(this Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<T> il2cpp) where T : unmanaged
    {
        if (il2cpp is null) throw new ArgumentNullException(nameof(il2cpp));
        
        T[] result = new T[il2cpp.Count];
        for (int i = 0; i < il2cpp.Length; i++)
            result[i] = il2cpp[i];
        return result;
    }
    
    public static List<T> ToManagedList<T>(this Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<T> il2cpp) where T : Il2CppObjectBase
    {
        if (il2cpp is null) throw new ArgumentNullException(nameof(il2cpp));

        List<T> result = new List<T>(il2cpp.Count);
        result.AddRange(il2cpp);
        return result;
    }
    
    public static List<T> ToManagedList<T>(this Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<T> il2cpp) where T : unmanaged
    {
        if (il2cpp is null) throw new ArgumentNullException(nameof(il2cpp));

        List<T> result = new List<T>(il2cpp.Count);
        result.AddRange(il2cpp);
        return result;
    }
    
    public static Il2CppReferenceArray<T> ToReferenceArray<T>(this IReadOnlyCollection<T> list) where T : Il2CppObjectBase
    {
        if (list is null) throw new ArgumentNullException(nameof(list));

        Il2CppReferenceArray<T> result = new Il2CppReferenceArray<T>(list.Count);
        Int32 index = 0;
        foreach (var item in list)
            result[index++] = item;
        return result;
    }
    
    public static Il2CppStructArray<T> ToStructArray<T>(this IReadOnlyCollection<T> list) where T : unmanaged
    {
        if (list is null) throw new ArgumentNullException(nameof(list));

        Il2CppStructArray<T> result = new Il2CppStructArray<T>(list.Count);
        Int32 index = 0;
        foreach (var item in list)
            result[index++] = item;
        return result;
    }
    
    public static List<T> ToManaged<T>(this Il2CppSystem.Collections.Generic.List<T> il2cpp)
    {
        if (il2cpp is null) throw new ArgumentNullException(nameof(il2cpp));

        List<T> result = new List<T>(il2cpp.Count);
        foreach (var item in il2cpp)
            result.Add(item);
        return result;
    }

    public static Dictionary<TKey1, Dictionary<TKey2, TValue>> ToManaged<TKey1, TKey2, TValue>(
        this Il2CppSystem.Collections.Generic.Dictionary<TKey1, Il2CppSystem.Collections.Generic.Dictionary<TKey2, TValue>> il2cpp)
    {
        return il2cpp.ToManaged(k => k, v => v.ToManaged());
    }

    public static Dictionary<TKey, TValue> ToManaged<TKey, TValue>(this Il2CppSystem.Collections.Generic.Dictionary<TKey, TValue> il2cpp)
    {
        return il2cpp.ToManaged(k => k, v => v);
    }

    public static Dictionary<TTargetKey, TTargetValue> ToManaged<TSourceKey, TSourceValue, TTargetKey, TTargetValue>(
        this Il2CppSystem.Collections.Generic.Dictionary<TSourceKey, TSourceValue> il2cpp,
        Func<TSourceKey, TTargetKey> keySelector,
        Func<TSourceValue, TTargetValue> valueSelector)
    {
        if (il2cpp is null) throw new ArgumentNullException(nameof(il2cpp));
        if (keySelector is null) throw new ArgumentNullException(nameof(keySelector));
        if (valueSelector is null) throw new ArgumentNullException(nameof(valueSelector));

        if (il2cpp.comparer.Pointer != Il2CppSystem.Collections.Generic.EqualityComparer<TSourceKey>.Default.Pointer)
            throw new ArgumentException($"The IL2CPP Dictionary uses a non-standard Comparer ([{il2cpp.comparer}]) that cannot be converted to a Managed type.", nameof(il2cpp));

        var result = new Dictionary<TTargetKey, TTargetValue>(il2cpp.Count);

        foreach ((TSourceKey k, TSourceValue v) in il2cpp)
            result.Add(keySelector(k), valueSelector(v));

        return result;
    }
}