using System;
using HarmonyLib;
using Last.Management;
using Memoria.FFPR.BeepInEx;

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

[HarmonyPatch(typeof(UserDataManager), nameof(UserDataManager.ToJSON))]
public sealed class UserDataManager_ToJSON : Il2CppSystem.Object
{
    public UserDataManager_ToJSON(IntPtr ptr) : base(ptr)
    {
    }

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