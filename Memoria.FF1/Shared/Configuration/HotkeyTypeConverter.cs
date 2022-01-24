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
        if (hotkey == default)
            return KeyCode.None.ToString();

        StringBuilder sb = new();
        if (hotkey.Control) sb.Append("Ctrl+");
        if (hotkey.Alt) sb.Append("Alt+");
        if (hotkey.Shift) sb.Append("Shift+");
        foreach (var modifier in hotkey.Modifiers)
        {
            sb.Append(modifier);
            sb.Append('+');
        }
        sb.Append(hotkey.Key);

        return sb.ToString();
    }

    private static Object ConvertToObject(String value, Type type)
    {
        CheckSupportedType(type);

        if (String.IsNullOrWhiteSpace(value))
            return new Hotkey { Key = KeyCode.None };

        String[] result = value.Split('+', '-');
        if (result.Length == 0)
            return new Hotkey { Key = KeyCode.None };

        KeyCode key = (KeyCode)Enum.Parse(typeof(KeyCode), result.Last());
        Boolean alt = false;
        Boolean shift = false;
        Boolean control = false;
        List<KeyCode> modifiers = new();
        for (Int32 i = 0; i < result.Length - 1; i++)
        {
            switch (result[i].ToLowerInvariant())
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
                    modifiers.Add((KeyCode)Enum.Parse(typeof(KeyCode), result[i]));
                    throw new FormatException(result[i]);
            }
        }

        return new Hotkey
        {
            Key = key,
            Control = control,
            Alt = alt,
            Shift = shift,
            Modifiers = modifiers.Count > 0 ? modifiers : Array.Empty<KeyCode>()
        };
    }

    private static void CheckSupportedType(Type type)
    {
        if (type != SupportedType)
            throw new NotSupportedException($"An unexpected type has occurred: {type.FullName}. Expected: {SupportedType.FullName}");
    }
}