using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Last.Battle;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.Core;

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.Update))]
public static class BattleProgressATB_Update
{
    private static readonly HashSet<IntPtr> LastReadyUnits = new();
    private static readonly List<IntPtr> CurrentReadyUnits = new();

    public static void Postfix(BattleProgressATB __instance)
    {
        if (!TurnBasedBattleState.IsTurnBased)
            return;
        
        BattlePlugManager manager = BattlePlugManager.Instance();

        var units = manager.BattleStatusControl.BattleStatusControlInfo.BattleUnitDataList;
        var atbDic = __instance.gaugeStatusDictionary.ToManaged();

        CurrentReadyUnits.Clear();
        
        foreach (BattleUnitData unit in units)
        {
            if (atbDic.TryGetValue(unit, out Single atbValue) && atbValue >= 100.0f)
                CurrentReadyUnits.Add(unit.Pointer);
        }

        if (LastReadyUnits.Count != CurrentReadyUnits.Count || CurrentReadyUnits.Any(u => !LastReadyUnits.Contains(u)))
        {
            LastReadyUnits.Clear();
            if (CurrentReadyUnits.Any(unit => !LastReadyUnits.Add(unit)))
                throw new Exception("if (!_readyUnits.Add(unit))");

            TurnBasedBattleState.ResetWaiting();
        }

        CurrentReadyUnits.Clear();
    }
}