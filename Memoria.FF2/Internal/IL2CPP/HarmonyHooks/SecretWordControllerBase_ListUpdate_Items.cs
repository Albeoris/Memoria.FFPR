using System;
using HarmonyLib;
using Il2CppSystem.Linq;
using Last.UI;
using Memoria.FF2.Internal.Core;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.Configuration.Scopes;
using Memoria.FFPR.IL2CPP;
using UnityEngine;
using UnityEngine.UI;

namespace Memoria.FF2.Internal.IL2CPP.HarmonyHooks;

// ReSharper disable once InconsistentNaming
[HarmonyPatch(typeof(SecretWordControllerBase), nameof(SecretWordControllerBase.ListUpdate),
    argumentTypes: new[]
    {
        typeof(InfiniteScrollView),
        typeof(Il2CppSystem.Collections.Generic.IEnumerable<ItemListContentData>)
    })]
public sealed class SecretWordControllerBase_ListUpdate_Items : Il2CppSystem.Object
{
    private static Color DefaultColor;

    public SecretWordControllerBase_ListUpdate_Items(IntPtr ptr) : base(ptr)
    {
    }

    // ReSharper disable once InconsistentNaming
    // ReSharper disable once IdentifierTypo
    public static void Postfix(SecretWordControllerBase __instance, InfiniteScrollView scrollview)
    {
        try
        {
            ColorNotUsedItems(__instance, scrollview);
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }

    private static void ColorNotUsedItems(SecretWordControllerBase controller, InfiniteScrollView scrollView)
    {
        if (controller == null)
            throw new ArgumentNullException(nameof(controller));
        if (scrollView == null)
            throw new ArgumentNullException(nameof(scrollView));

        ConversationConfiguration options = ModComponent.Instance.Config.Conversation;
        if (!options.ColorKeyItems)
            return;

        Color unusedColor = options.UnusedKeyItemsColor.Value;
        Color usedColor = options.UsedKeyItemsColor.Value;

        for (var index = 0; index < scrollView.objects.Count; index++)
        {
            RectTransform obj = scrollView.objects[index];
            if (!obj.gameObject.activeInHierarchy)
                continue;

            if (obj.name != "list_content")
                continue;

            ItemListContentData item = controller.itemDataList.ElementAt(index);
            Color color = ConversationManager.WasUsedItem(item.ItemId)
                ? usedColor
                : unusedColor;
            
            Text text = obj.GetComponentInChildren<Text>();
            if (text is null)
                continue;
            
            if (DefaultColor == default)
                DefaultColor = text.color;

            if (color == default)
                color = DefaultColor;

            text.color = color;
        }
    }
}