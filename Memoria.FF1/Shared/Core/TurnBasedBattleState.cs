using System;
using Last.Management;
using Memoria.FFPR.IL2CPP;

namespace Memoria.FFPR.Core;

public static class TurnBasedBattleState
{
    public static Boolean Waiting { get; private set; } = true;

    public static Boolean IsTurnBased
    {
        get
        {
            SystemConfig system = SystemConfig.Instance();
            if (system.BattleType != BattleType.ATB)
                return false;

            if (system.ATBBattleType != ATBBattleType.Wait)
                return false;

            return ModComponent.Instance.Config.BattleAtb.TurnBased.Value;
        }
    }

    public static void NextTurn()
    {
        Waiting = false;
    }

    public static void ResetWaiting()
    {
        Waiting = true;
    }
}