using System;
using System.Text.RegularExpressions;
using HarmonyLib;
using Last.Management;

namespace Memoria.FF6.Internal.IL2CPP.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(MessageManager), nameof(MessageManager.GetMessage))]
public static class MessageManager_GetMessage
{
    public const String SkipPrefix = "Skip.MessageManager_GetMessage.";
    
    private static readonly Regex NumberRegex = new("[0-9]+", RegexOptions.Compiled);
    
    public static Boolean Prefix(ref String key, ref String __result, ref String __state)
    {
        if (key is null)
            return true;

        if (TryHandleFormattedString(key, ref __result))
            return false;

        Int32 index = key.IndexOf(" -> ", StringComparison.Ordinal);
        if (index < 0)
            return true;

        __state = key.Substring(index + " -> ".Length);
        key = key.Substring(0, index);
        return true;
    }

    private static Boolean TryHandleFormattedString(String key, ref String __result)
    {
        if (key.StartsWith(SkipPrefix))
        {
            __result = key.Substring(SkipPrefix.Length);
            return true;
        }

        return false;
    }

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