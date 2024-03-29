﻿using System;
using HarmonyLib;
using Last.Defaine;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.IL2CPP;
using Serial.FF6.Management;

namespace Memoria.FF6.Internal.IL2CPP.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(JobInfomationData), nameof(JobInfomationData.GetSpecialAbilityCommandList))]
public static class JobInfomationData_GetSpecialAbilityCommandList
{
    public static void Postfix(ref Il2CppSystem.Collections.Generic.List<SpecialAbilityCommandId> __result)
    {
        try
        {
            if (__result is null || __result.Count < 1)
                return;
            
            if (!ModComponent.Instance.Config.BattleBlitz.EasyInput)
                return;

            __result = new();
            __result.Add(SpecialAbilityCommandId.ActionKey);
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }
}