using System;
using HarmonyLib;
using Last.Data.Master;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.IL2CPP;

namespace Memoria.FF6.Internal.IL2CPP.HarmonyHooks;

// ReSharper disable once InconsistentNaming
[HarmonyPatch(typeof(MasterManager), nameof(MasterManager.UpdateParse) )]
public sealed class MasterManager_MasterParse : Il2CppSystem.Object
{
    public MasterManager_MasterParse(IntPtr ptr) : base(ptr)
    {
    }

    public static void Prefix(MasterManager __instance)
    {
        try
        {
            // All .csv are loaded and can be changed here
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }
    
    public static void Postfix(MasterManager __instance)
    {
        try
        {
            // All master data parsed and can be changed here
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }
}