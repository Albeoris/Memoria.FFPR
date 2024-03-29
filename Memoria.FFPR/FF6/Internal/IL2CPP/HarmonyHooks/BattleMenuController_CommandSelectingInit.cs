﻿using System;
using HarmonyLib;
using Last.UI.KeyInput;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.FF6.Internal;
using Memoria.FFPR.IL2CPP;

namespace Memoria.FF6.Internal.IL2CPP.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(BattleMenuController), nameof(BattleMenuController.CommandSelectingInit))]
public static class BattleMenuController_CommandSelectingInit
{
    public static void Postfix(BattleMenuController __instance)
    {
        try
        {
            LeapAbilityColorization.Instance.AfterBattleCommandSelectingInit(__instance);
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }
}