using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.Core;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using UnityEngine.U2D;

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

[HarmonyPatch(typeof(SpriteAtlas), nameof(SpriteAtlas.GetSprites), typeof(Il2CppReferenceArray<Sprite>))]
public sealed class SpriteAtlas_GetSprites
{
    public static Boolean Prefix(ref Il2CppReferenceArray<Sprite> sprites, SpriteAtlas __instance, ref Int32 __result)
    {
        if (!SpriteAtlasCache.FindAtlas(__instance.name, out var atlas))
            return true;

        IReadOnlyList<Sprite> list = atlas.Sprites;
        Int32 min = Math.Min(sprites.Length, list.Count);

        for (Int32 i = 0; i < min; i++)
            sprites[i] = list[i];

        __result = min;
        return false;
    }

    // Debug:
    // public static void Postfix(ref Il2CppReferenceArray<Sprite> sprites, SpriteAtlas __instance, ref Int32 __result)
    // {
    //     if (!SpriteAtlasCache.FindAtlas(__instance.name, out var atlas))
    //         return;
    //
    //     IReadOnlyList<Sprite> list = atlas.Sprites;
    //     Int32 min = Math.Min(sprites.Length, list.Count);
    //
    //     using (var sw = File.AppendText(@"D:\1.txt"))
    //     {
    //         sw.WriteLine("========");
    //         sw.WriteLine(__instance.name);
    //         foreach (var path in atlas._usedPaths)
    //             sw.WriteLine(path);
    //         sw.WriteLine($"Desired: {sprites.Count}, Existing: {list.Count}");
    //
    //         for (Int32 i = 0; i < min; i++)
    //         {
    //             Sprite old = sprites[i];
    //
    //             sw.WriteLine($"Old: {old.name}");
    //             sw.WriteLine($"\tRect: {old.rect}");
    //             sw.WriteLine($"\ttextureRect: {old.GetTextureRect()}");
    //             sw.WriteLine($"\tpivot: {old.pivot}");
    //             sw.WriteLine($"\tborder: {old.border}");
    //             sw.WriteLine($"\tbounds: {old.bounds}");
    //             sw.WriteLine($"\tvertices: {String.Join("|", old.vertices.ToManaged())}");
    //             sw.WriteLine($"\ttriangles: {String.Join("|", old.triangles.ToManaged())}");
    //
    //             var cur = list[i];
    //             sw.WriteLine($"Cur: {cur.name}");
    //             sw.WriteLine($"\tRect: {cur.rect}");
    //             sw.WriteLine($"\ttextureRect: {cur.GetTextureRect()}");
    //             sw.WriteLine($"\tpivot: {cur.pivot}");
    //             sw.WriteLine($"\tborder: {cur.border}");
    //             sw.WriteLine($"\tbounds: {cur.bounds}");
    //             sw.WriteLine($"\tvertices: {String.Join("|", cur.vertices.ToManaged())}");
    //             sw.WriteLine($"\ttriangles: {String.Join("|", cur.triangles.ToManaged())}");
    //             sw.WriteLine();
    //             sprites[i] = list[i];
    //         }
    //
    //         __result = min;
    //         return;
    //     }
    //}
}