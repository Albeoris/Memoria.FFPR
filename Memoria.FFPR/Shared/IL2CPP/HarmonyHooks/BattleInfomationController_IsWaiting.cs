using System;
using HarmonyLib;
using Last.UI.KeyInput;
using Memoria.FFPR.Core;

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

[HarmonyPatch(typeof(BattleInfomationController), nameof(BattleInfomationController.IsWaiting))]
public sealed class BattleInfomationController_IsWaiting
{
    public static void Postfix(BattleInfomationController __instance, ref Boolean __result)
    {
        if (__result || !TurnBasedBattleState.IsTurnBased)
            return;
        
        if (__instance.stateMachine.Current == BattleInfomationController.State.CommandSelect)
            __result = TurnBasedBattleState.Waiting;
    }
}