using System;
using HarmonyLib;
using Memoria.FFPR.Core;
using UnhollowerBaseLib;
using UnityEngine;
using UnityEngine.U2D;

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

[HarmonyPatch(typeof(SpriteAtlas), nameof(SpriteAtlas.GetSprites), typeof(Il2CppReferenceArray<Sprite>), typeof(String))]
public sealed class SpriteAtlas_GetSprites_WithName
{
    public static Boolean Prefix(ref Il2CppReferenceArray<Sprite> sprites, String name, SpriteAtlas __instance, ref Int32 __result)
    {
        if (!SpriteAtlasCache.FindAtlas(__instance.name, out var atlas))
            return true;

        if (sprites.Length < 1)
            return true;

        if (!atlas.FindSprite(name, out var sprite))
            return true;

        sprites[0] = sprite;
        __result = 1;
        return false;
    }
}