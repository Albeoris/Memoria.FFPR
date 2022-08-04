using System;
using HarmonyLib;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.FF6.Internal;
using Memoria.FFPR.IL2CPP;
using Serial.FF6.UI.KeyInput;

namespace Memoria.FF6.Internal.IL2CPP.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(AbilityContentListController), nameof(AbilityContentListController.SetUseValue))]
public sealed class AbilityContentListController_SetUseValue : Il2CppSystem.Object
{
    public AbilityContentListController_SetUseValue(IntPtr ptr) : base(ptr)
    {
    }
    
    public static void Postfix(AbilityContentListController __instance)
    {
        try
        {
            RageAbilityColorization.Instance.AfterAbilityListUpdateView(__instance);
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }
}