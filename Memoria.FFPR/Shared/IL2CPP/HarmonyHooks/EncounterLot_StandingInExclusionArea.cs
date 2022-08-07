using System;
using HarmonyLib;
using Last.Map;

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(EncounterLot), nameof(EncounterLot.StandingInExclusionArea))]
public static class EncounterLot_StandingInExclusionArea
{
    public static Boolean Prefix(ref Boolean __result)
    {
        if (ModComponent.Instance.EncountersControl.DisableEncounters)
        {
            __result = true;

            // Skip native method
            return false;
        }

        // Call native method
        return true;
    }
}