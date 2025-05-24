using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Configuration;
using UnityEngine;
using Object = System.Object;

namespace Memoria.FFPR.Configuration;

public static class HotkeyTypeConverter
{
    private static Type SupportedType { get; } = typeof(Hotkey);

    public static void Init()
    {
        if (!TomlTypeConverter.CanConvert(SupportedType))
        {
            TypeConverter converter = new() { ConvertToObject = ConvertToObject, ConvertToString = ConvertToString };
            TomlTypeConverter.AddConverter(SupportedType, converter);
        }
    }

    private static String ConvertToString(Object boxed, Type type)
    {
        CheckSupportedType(type);

        Hotkey hotkey = (Hotkey)boxed;
        return ConvertToString(hotkey);
    }

    public static String ConvertToString(Hotkey hotkey)
    {
        if (hotkey == default)
            return KeyCode.None.ToString();

        StringBuilder sb = new();
        if (hotkey.Control) sb.Append("Ctrl+");
        if (hotkey.Alt) sb.Append("Alt+");
        if (hotkey.Shift) sb.Append("Shift+");
        foreach (var modifier in hotkey.ModifierKeys)
        {
            sb.Append(modifier);
            sb.Append('+');
        }

        foreach (var modifier in hotkey.ModifierActions)
        {
            sb.Append('[');
            sb.Append(modifier);
            sb.Append(']');
            sb.Append('+');
        }

        if (!String.Equals(hotkey.Action, "None", StringComparison.InvariantCultureIgnoreCase))
        {
            sb.Append('[');
            sb.Append(hotkey.Action);
            sb.Append(']');
        }
        else
            sb.Append(hotkey.Key);

        if (hotkey.MustHeld)
            sb.Append("(Hold)");

        return sb.ToString();
    }

    private static Object ConvertToObject(String value, Type type)
    {
        CheckSupportedType(type);

        return ConvertToObject(value);
    }

    public static Hotkey ConvertToObject(String value)
    {
        if (String.IsNullOrWhiteSpace(value))
            return new Hotkey(KeyCode.None);

        String[] split = value
            .Split('+', '-')
            .Select(v => v.Trim())
            .ToArray();

        if (split.Length == 0)
            return new Hotkey(KeyCode.None);

        String last = split.Last();
        Boolean mustHeld = CheckMustHeld(ref last);

        if (last.Length == 0)
            return new Hotkey(KeyCode.None);

        ParseKeyOrAction(last, out KeyCode key, out String action);

        Boolean alt = false;
        Boolean shift = false;
        Boolean control = false;
        List<KeyCode> modifierKeys = new();
        List<String> modifierActions = new();
        for (Int32 i = 0; i < split.Length - 1; i++)
        {
            switch (split[i].ToLowerInvariant())
            {
                case "ctrl":
                case "control":
                    control = true;
                    break;
                case "alt":
                    alt = true;
                    break;
                case "shift":
                    shift = true;
                    break;
                default:
                    ParseKeyOrAction(split[i], out var modKey, out var modAction);
                    if (modKey != KeyCode.None)
                        modifierKeys.Add(modKey);
                    if (!String.Equals(modAction, "None", StringComparison.InvariantCultureIgnoreCase))
                        modifierActions.Add(modAction);
                    break;
            }
        }

        Hotkey result;
        if (!String.Equals(action, "None", StringComparison.InvariantCultureIgnoreCase))
            result = new Hotkey(action);
        else
            result = new Hotkey(key);

        result.MustHeld = mustHeld;
        result.Control = control;
        result.Alt = alt;
        result.Shift = shift;
        result.ModifierKeys = modifierKeys.Count > 0 ? modifierKeys : Array.Empty<KeyCode>();
        result.ModifierActions = modifierActions.Count > 0 ? modifierActions : Array.Empty<String>();

        return result;
    }

    private static void ParseKeyOrAction(String last, out KeyCode key, out String action)
    {
        if (last[0] == '[' && last[last.Length - 1] == ']')
        {
            key = KeyCode.None;
            action = last.Substring(1, last.Length - 2);
        }
        else
        {
            key = (KeyCode)Enum.Parse(typeof(KeyCode), last);
            action = "None";
        }
    }

    private static Boolean CheckMustHeld(ref String last)
    {
        if (last.EndsWith("(Hold)", StringComparison.InvariantCultureIgnoreCase))
        {
            last = last.Substring(0, last.Length - "(Hold)".Length).Trim();
            return true;
        }

        return false;
    }

    private static void CheckSupportedType(Type type)
    {
        if (type != SupportedType)
            throw new NotSupportedException($"An unexpected type has occurred: {type.FullName}. Expected: {SupportedType.FullName}");
    }
}