using System;
using HarmonyLib;
using Last.Management;
using Memoria.FFPR.BeepInEx;

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

// ReSharper disable once InconsistentNaming
[HarmonyPatch(typeof(UserDataManager), nameof(UserDataManager.FromJson))]
public sealed class UserDataManager_FromJson : Il2CppSystem.Object
{
    public UserDataManager_FromJson(IntPtr ptr) : base(ptr)
    {
    }

    // ReSharper disable once InconsistentNaming
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