using System;
using HarmonyLib;
using Last.Data.User;
using Last.Systems;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.IL2CPP;

namespace Memoria.FF6.Internal.IL2CPP.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(OwnedAbility), nameof(OwnedAbility.MesIdDescription), MethodType.Getter)]
public sealed class OwnedAbility_GetMesIdDescription : Il2CppSystem.Object
{
    public OwnedAbility_GetMesIdDescription(IntPtr ptr) : base(ptr)
    {
    }

    public static void Postfix(OwnedAbility __instance, ref String __result)
    {
        try
        {
            // Don't change existing description
            if (__result != "None")
                return;
            
            if (!ModComponent.Instance.Config.BattleGau.GenerateRageAbilityDescription)
                return;

            String desc = ContentUtitlity.GetAbilityDescription(__instance.Ability);
            if (!String.IsNullOrEmpty(desc))
                __result = $"{MessageManager_GetMessage.SkipPrefix}{ContentUtitlity.GetAbilityDescription(__instance.Ability)}";
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }
}