using System;
using HarmonyLib;
using Last.Data.Master;
using Last.Map;
using Memoria.FFPR.IL2CPP;
using Serial.Template.Management;
using Boolean = System.Boolean;
using IntPtr = System.IntPtr;
// ReSharper disable InconsistentNaming

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

[HarmonyPatch(typeof(EncountTable), nameof(EncountTable.GetEncountData))]
public static class EncountTable_GetEncountData
{
    public static Boolean Prefix(ref MonsterParty __result)
    {
        if (ModComponent.Instance.EncountersControl.DisableEncounters)
        {
            __result = null;

            // Skip native method
            return false;
        }

        // Call native method
        return true;
    }
}