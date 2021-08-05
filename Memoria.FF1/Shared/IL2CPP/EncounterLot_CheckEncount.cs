using System;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Last.Management;
using Last.Map;
using Memoria.FFPR.Configuration;
using Memoria.FFPR.Core;
using UnhollowerBaseLib;
using UnityEngine;
using Boolean = System.Boolean;
using Exception = System.Exception;
using File = System.IO.File;
using IntPtr = System.IntPtr;
using Object = Il2CppSystem.Object;
using Path = System.IO.Path;
using String = System.String;

namespace Memoria.FFPR.IL2CPP
{
    [HarmonyPatch(typeof(EncounterLot), nameof(EncounterLot.CheckEncount))]
    public sealed class EncounterLot_CheckEncount : Il2CppSystem.Object
    {
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
}