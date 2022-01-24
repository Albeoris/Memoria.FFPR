using System;
using Last.Management;
using Last.Systems.Indicator;
using Memoria.FFPR.IL2CPP;
using UnityEngine;

namespace Memoria.FFPR.Core;

public static class SystemIndicatorAccessor
{
    public static void Show(String messageKey, Single time, Color foreground, Color outline)
    {
        SystemIndicator indicator = SystemIndicator.Instance;

        var mode = SystemIndicator.Mode.kTitleLoading;
        var modeIndex = (Int32)mode;
        var property = SystemIndicator.s_text_properties[modeIndex];
        var nativeKey = property.msgid;
        property.msgid = messageKey;
        try
        {
            SystemIndicator.s_text_properties[modeIndex] = property;
            indicator.Show(mode);
        }
        finally
        {
            property.msgid = nativeKey;
            SystemIndicator.s_text_properties[modeIndex] = property;
        }

        indicator.text.color = foreground;
        indicator.outline.effectColor = outline;
        indicator.outline.effectDistance = Vector2.one * 5;
        indicator.Invoke(nameof(indicator.Hide), time);
    }
}