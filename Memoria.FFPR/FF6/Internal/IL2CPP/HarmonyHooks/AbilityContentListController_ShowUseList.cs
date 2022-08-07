using System;
using HarmonyLib;
using Last.Data.Master;
using Last.Data.User;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.FF6.Internal;
using Memoria.FFPR.IL2CPP;
using Serial.FF6.UI.KeyInput;

namespace Memoria.FF6.Internal.IL2CPP.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(AbilityContentListController), nameof(AbilityContentListController.ShowUseList))]
public static class AbilityContentListController_ShowUseList
{
    public static void Prefix(AbilityContentListController __instance, Command command, OwnedCharacterData data)
    {
        try
        {
            RageAbilityColorization.Instance.BeforeAbilityListShown(__instance, command, data);
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }
    
    public static void Postfix(AbilityContentListController __instance, Command command, OwnedCharacterData data)
    {
        try
        {
            RageAbilityColorization.Instance.AfterAbilityListShown(__instance, command, data);
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }
}