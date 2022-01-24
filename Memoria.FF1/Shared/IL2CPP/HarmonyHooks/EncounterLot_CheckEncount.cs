using HarmonyLib;
using Last.Map;
using Memoria.FFPR.IL2CPP;
using Boolean = System.Boolean;
using IntPtr = System.IntPtr;
// ReSharper disable InconsistentNaming

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

[HarmonyPatch(typeof(EncounterLot), nameof(EncounterLot.CheckEncount))]
public sealed class EncounterLot_CheckEncount : Il2CppSystem.Object
{
    // ReSharper disable once IdentifierTypo
    public EncounterLot_CheckEncount(IntPtr ptr) : base(ptr)
    {
    }

    public static Boolean Prefix(ref Boolean __result)
    {
        if (ModComponent.Instance.EncountersControl.DisableEncounters)
        {
            __result = false;
                
            // Skip native method
            return false;
        }

        // Call native method
        return true;
    }
}