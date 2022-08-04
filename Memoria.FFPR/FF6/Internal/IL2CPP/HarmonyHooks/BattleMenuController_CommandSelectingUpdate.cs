using System;
using HarmonyLib;
using Last.UI.KeyInput;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.FF6.Internal;
using Memoria.FFPR.IL2CPP;

namespace Memoria.FF6.Internal.IL2CPP.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(BattleMenuController), nameof(BattleMenuController.CommandSelectingUpdate))]
public sealed class BattleMenuController_CommandSelectingUpdate : Il2CppSystem.Object
{
    public BattleMenuController_CommandSelectingUpdate(IntPtr ptr) : base(ptr)
    {
    }

    public static void Postfix(BattleMenuController __instance)
    {
        try
        {
            LeapAbilityColorization.Instance.AfterBattleCommandSelectingUpdate(__instance);
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }
}