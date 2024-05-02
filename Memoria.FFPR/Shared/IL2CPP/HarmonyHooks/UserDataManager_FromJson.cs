using System;
using HarmonyLib;
using Last.Management;
using Memoria.FFPR.BeepInEx;

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

#if FF5 || FF6
// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(UserDataManager), nameof(UserDataManager.FromJson))]
public static class UserDataManager_FromJson
{
    public static void Postfix(String json)
    {
        try
        {
            SaveManager.LoadData(json);
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }
}
#endif