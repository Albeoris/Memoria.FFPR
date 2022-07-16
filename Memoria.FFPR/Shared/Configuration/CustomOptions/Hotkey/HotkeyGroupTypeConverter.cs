using System;
using System.Linq;
using BepInEx.Configuration;
using UnityEngine;
using Object = System.Object;

namespace Memoria.FFPR.Configuration;

public static class HotkeyGroupTypeConverter
{
    private static Type SupportedType { get; } = typeof(HotkeyGroup);

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

        HotkeyGroup hotkey = (HotkeyGroup)boxed;
        return ConvertToString(hotkey);
    }

    public static String ConvertToString(HotkeyGroup hotkey)
    {
        if (hotkey == default || hotkey.Keys.Count == 0)
            return KeyCode.None.ToString();

        return String.Join("; ", hotkey.Keys.Select(HotkeyTypeConverter.ConvertToString));
    }

    private static Object ConvertToObject(String value, Type type)
    {
        CheckSupportedType(type);

        return ConvertToObject(value);
    }

    public static HotkeyGroup ConvertToObject(String value)
    {
        if (String.IsNullOrWhiteSpace(value))
            return HotkeyGroup.None;

        String[] split = value
            .Split(new[]{';', ','}, StringSplitOptions.RemoveEmptyEntries)
            .Select(v => v.Trim())
            .ToArray();

        if (split.Length == 0)
            return HotkeyGroup.None;

        return HotkeyGroup.Create(split.Select(HotkeyTypeConverter.ConvertToObject).ToArray());
    }

    private static void CheckSupportedType(Type type)
    {
        if (type != SupportedType)
            throw new NotSupportedException($"An unexpected type has occurred: {type.FullName}. Expected: {SupportedType.FullName}");
    }
}