using System;
using HarmonyLib;
using Il2CppSystem.Input;
using Last.Map;
using Last.UI;
using IS = UnityEngine.InputSystem;
using static FieldMap;

namespace Memoria.FFPR.Shared.IL2CPP.HarmonyHooks
{
    [HarmonyPatch(typeof(FieldMap), nameof(FieldMap.LateUpdate))]
    public static class FieldMap_LateUpdate
    {
        //This might be overkill... But it works.
        public static void Prefix(FieldMap __instance)
        {
            //TODO: add configuratin option.
            if (__instance.keyAutoDash != null)
            {
                __instance.keyAutoDash = null;
            }
        }
    }

    //[HarmonyPatch(typeof(MapUIManager), nameof(MapUIManager.AutoDashOperationSwitch))]
    //public static class MapUIManager_AutoDashOperationSwitch
    //{
    //    //This is almost right (no visual indicator now, but effect still happens)
    //    public static bool Prefix()
    //    {
    //        //Skip native method.
    //        return false;
    //    }
    //}

    
}
