using System;
using HarmonyLib;
using Memoria.FFPR.Core;
using UnityEngine;
using UnityEngine.U2D;

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

[HarmonyPatch(typeof(SpriteAtlas),nameof(SpriteAtlas.GetSprite))]
public sealed class SpriteAtlas_GetSprite
{
    public static Boolean Prefix(String name, SpriteAtlas __instance, ref Sprite __result)
    {
        if (!SpriteAtlasCache.FindAtlas(__instance.name, out var atlas))
            return true;

        if (atlas.FindSprite(name, out __result))
            return false;

        return true;
    }
}