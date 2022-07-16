using System;
using System.Text.RegularExpressions;
using HarmonyLib;
using Last.Management;

namespace Memoria.FF6.Internal.IL2CPP.HarmonyHooks;

// ReSharper disable once InconsistentNaming
[HarmonyPatch(typeof(MessageManager), nameof(MessageManager.GetMessage))]
public sealed class MessageManager_GetMessage : Il2CppSystem.Object
{
    private static readonly Regex NumberRegex = new("[0-9]+", RegexOptions.Compiled);
    
    public MessageManager_GetMessage(IntPtr ptr) : base(ptr)
    {
    }
    
    // ReSharper disable once InconsistentNaming
    public static void Prefix(ref String key, ref String __result, ref String __state)
    {
        if (key is null)
            return;

        Int32 index = key.IndexOf(" -> ", StringComparison.Ordinal);
        if (index < 0)
            return;

        __state = key.Substring(index + " -> ".Length);
        key = key.Substring(0, index);
    }

    // ReSharper disable once InconsistentNaming
    public static void Postfix(String key, ref String __result, ref String __state)
    {
        if (__state is null)
            return;
        
        if (__result is null)
            return;

        if (String.IsNullOrWhiteSpace(__result))
        {
            __result += __state;
            return;
        }

        __result = NumberRegex.Replace(__result, __state);
    }
}