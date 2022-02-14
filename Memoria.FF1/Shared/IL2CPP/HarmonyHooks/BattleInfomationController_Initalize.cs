using HarmonyLib;
using Last.UI.KeyInput;
using Memoria.FFPR.Core;

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

[HarmonyPatch(typeof(BattleInfomationController), nameof(BattleInfomationController.Reset))]
public sealed class BattleInfomationController_Reset
{
    public static void Prefix()
    {
        // Can be called from BattleController_StartBattle
        TurnBasedBattleState.ResetWaiting();
    }
}