using System;
using HarmonyLib;
using Memoria.FFPR.Core;
using UnityEngine.U2D;

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

[HarmonyPatch(typeof(SpriteAtlas), nameof(SpriteAtlas.spriteCount), MethodType.Getter)]
public sealed class SpriteAtlas_spriteCount
{
    public static Boolean Prefix(SpriteAtlas __instance, ref Int32 __result)
    {
        if (!SpriteAtlasCache.FindAtlas(__instance.name, out var atlas))
            return true;

        __result = atlas.Sprites.Count;
        return false;
    }
}