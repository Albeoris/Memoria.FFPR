﻿using System;
using System.Collections.Generic;
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

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(SecretWordControllerBase), nameof(SecretWordControllerBase.ListUpdate),
    argumentTypes: new[]
    {
        typeof(InfiniteScrollView),
        typeof(Il2CppSystem.Collections.Generic.IEnumerable<SelectFieldContentManager.SelectFieldContentData>)
    })]
public static class SecretWordControllerBase_ListUpdate_Words
{
    private static Color _defaultColor;

    // ReSharper disable once IdentifierTypo
    public static void Postfix(SecretWordControllerBase __instance, InfiniteScrollView scrollview, IEnumerable<SelectFieldContentManager.SelectFieldContentData> contents)
    {
        try
        {
            ColorNotUsedWords(__instance, scrollview);
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }

    private static void ColorNotUsedWords(SecretWordControllerBase controller, InfiniteScrollView scrollView)
    {
        if (controller == null)
            throw new ArgumentNullException(nameof(controller));
        if (scrollView == null)
            throw new ArgumentNullException(nameof(scrollView));
        
        ConversationConfiguration options = ModComponent.Instance.Config.Conversation;
        if (!options.ColorWords)
            return;
        
        Color unusedColor = options.UnusedWordsColor.Value;
        Color usedColor = options.UsedWordsColor.Value;
        
        for (var index = 0; index < scrollView.objects.Count; index++)
        {
            RectTransform obj = scrollView.objects[index];
            if (!obj.gameObject.activeInHierarchy)
                continue;

            if (obj.name != "list_content")
                continue;

            SelectFieldContentManager.SelectFieldContentData word = controller.wordDataList.ElementAt(index);
            Color color = ConversationManager.WasUsedWord(word.UniqueId)
                ? usedColor
                : unusedColor;

            Text text = obj.GetComponentInChildren<Text>();
            if (text is null)
                continue;
            
            if (_defaultColor == default)
                _defaultColor = text.color;

            if (color == default)
                color = _defaultColor;

            text.color = color;
        }
    }
}