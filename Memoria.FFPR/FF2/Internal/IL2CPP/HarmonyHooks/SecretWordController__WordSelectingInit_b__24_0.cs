using System;
using HarmonyLib;
using Il2CppSystem.Linq;
using Last.UI;
using Last.UI.KeyInput;
using Memoria.FF2.Internal.Core;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.IL2CPP;

namespace Memoria.FF2.Internal.IL2CPP.HarmonyHooks;

// ReSharper disable once InconsistentNaming
[HarmonyPatch(typeof(SecretWordController), nameof(SecretWordController._WordSelectingInit_b__24_0))]
public sealed class SecretWordController__WordSelectingInit_b__24_0 : Il2CppSystem.Object
{
    public SecretWordController__WordSelectingInit_b__24_0(IntPtr ptr) : base(ptr)
    {
    }

    // ReSharper disable once InconsistentNaming
    public static void Prefix(SecretWordController __instance, int index)
    {
        try
        {
            RememberUsedWord(__instance, index);
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }

    private static void RememberUsedWord(SecretWordController controller, Int32 index)
    {
        if (controller == null)
            throw new ArgumentNullException(nameof(controller));
        
        SelectFieldContentManager.SelectFieldContentData word = controller.wordDataList.ElementAt(index);
        ConversationManager.RememberUsedWord(word.UniqueId);
    }
}