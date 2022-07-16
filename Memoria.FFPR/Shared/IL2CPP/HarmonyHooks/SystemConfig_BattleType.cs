using System;
using HarmonyLib;
using Last.Management;
using Memoria.FFPR.Configuration.Scopes;

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

[HarmonyPatch(typeof(SystemConfig), nameof(SystemConfig.BattleType), MethodType.Getter)]
public static class SystemConfig_BattleType
{
    public static Boolean Prefix(ref BattleType __result)
    {
        BattleSystemConfiguration config = ModComponent.Instance.Config.BattleSystem;
        if (!config.ChangeBattleType.Value)
            return true; // Call native method

        __result = config.BattleType.Value;

        // Skip native method
        return false;
    }
}