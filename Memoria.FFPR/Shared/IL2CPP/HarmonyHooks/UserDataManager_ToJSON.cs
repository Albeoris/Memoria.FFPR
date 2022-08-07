using System;
using HarmonyLib;
using Last.Management;
using Memoria.FFPR.BeepInEx;

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(UserDataManager), nameof(UserDataManager.ToJSON))]
public static class UserDataManager_ToJSON
{
    public static void Postfix(ref string __result)
    {
        try
        {
            __result = SaveManager.SaveData(__result);
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }
}