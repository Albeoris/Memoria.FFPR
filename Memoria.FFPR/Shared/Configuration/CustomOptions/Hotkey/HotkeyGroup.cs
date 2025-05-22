using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = System.Object;

namespace Memoria.FFPR.Configuration;

using Memoria.FFPR.BeepInEx;

public sealed class HotkeyGroup
{
    public static HotkeyGroup None => Create(Hotkey.None);

    public IReadOnlyList<Hotkey> Keys { get; }
    public IReadOnlyList<IReadOnlyList<Hotkey>> GroupedByHeld { get; }

    private HotkeyGroup(IReadOnlyList<Hotkey> keys)
    {
        keys = keys.DistinctBy(HotkeyTypeConverter.ConvertToString);
        
        Keys = keys;
        GroupedByHeld = GroupKeysByHeld(keys);
    }

    public static HotkeyGroup Create(Hotkey key)
    {
        return new HotkeyGroup(new[] { key });
    }

    public static HotkeyGroup Create(IReadOnlyList<Hotkey> keys)
    {
        return keys.Count == 0 ? None : new HotkeyGroup(keys);
    }

    private static IReadOnlyList<IReadOnlyList<Hotkey>> GroupKeysByHeld(IReadOnlyList<Hotkey> keys)
    {
        return keys
            .GroupBy(GetHotkeyId)
            .Select(g => g.OrderByDescending(h => h.MustHeld).ToArray())
            .ToArray();

        String GetHotkeyId(Hotkey h)
        {
            Boolean mustHeld = h.MustHeld;
            h.MustHeld = false;
            String str = HotkeyTypeConverter.ConvertToString(h);
            h.MustHeld = mustHeld;
            return str;
        }
    }
}