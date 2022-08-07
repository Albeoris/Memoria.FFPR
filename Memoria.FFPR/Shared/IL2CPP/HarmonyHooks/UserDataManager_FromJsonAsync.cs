using System;
using HarmonyLib;
using Last.Management;
using Memoria.FFPR.BeepInEx;

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(UserDataManager), nameof(UserDataManager.FromJsonAsync))]
public static class UserDataManager_FromJsonAsync
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