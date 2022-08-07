using System;
using System.Threading;
using HarmonyLib;
using Last.Management;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.Configuration.Scopes;
using Memoria.FFPR.IL2CPP;

namespace Memoria.FF6.Internal.IL2CPP.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(OwnedCharacterClient), nameof(OwnedCharacterClient.AddFromJson))]
public static class OwnedCharacterClient_AddFromJson
{
    public static Int64 IsInvoked;
    
    public static void Prefix(OwnedCharacterClient __instance)
    {
        try
        {
            if (ModComponent.Instance.Config.BattleGau.RageAbilitiesSortOrder == AbilityOrder.Default)
                return;
            
            Int64 value = Interlocked.Exchange(ref IsInvoked, 1);
            if (value != 0)
                throw new Exception("Interlocked.Exchange(ref IsInvoked, 1) != 0");
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }
    
    public static void Postfix(OwnedCharacterClient __instance)
    {
        try
        {
            if (ModComponent.Instance.Config.BattleGau.RageAbilitiesSortOrder == AbilityOrder.Default)
                return;
            
            Int64 value = Interlocked.Exchange(ref IsInvoked, 0);
            if (value != 1)
                throw new Exception("Interlocked.Exchange(ref IsInvoked, 0) != 1");
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }
}