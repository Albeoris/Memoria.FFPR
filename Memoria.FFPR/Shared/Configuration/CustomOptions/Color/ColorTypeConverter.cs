using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;
using UnityEngine;
using Object = System.Object;

namespace Memoria.FFPR.Configuration;

public static class ColorTypeConverter
{
    private static Type SupportedType { get; } = typeof(Color);
    private static String NoneColorName { get; } = "None";

    private static readonly Dictionary<String, Color> ColorsByName = new();
    private static readonly Dictionary<Color, String> NamesByColor = new();

    public static String ColorNames { get; private set; } = $"#RRGGBBAA, {NoneColorName}";

    public static void Init()
    {
        if (!TomlTypeConverter.CanConvert(SupportedType))
        {
            TypeConverter converter = new() { ConvertToObject = ConvertToObject, ConvertToString = ConvertToString };
            TomlTypeConverter.AddConverter(SupportedType, converter);
            InitializeColorNames();
        }
    }

    private static String ConvertToString(Object boxed, Type type)
    {
        CheckSupportedType(type);

        Color color = (Color)boxed;
        if (color == default)
            return NoneColorName;

        return NamesByColor.TryGetValue(color, out var name)
            ? name
            : ToHex(color);
    }

    private static Object ConvertToObject(String value, Type type)
    {
        CheckSupportedType(type);

        if (value == NoneColorName)
            return default(Color);

        if (ColorsByName.TryGetValue(value, out var color))
            return color;

        return FromHex(value);
    }

    private static void CheckSupportedType(Type type)
    {
        if (type != SupportedType)
            throw new NotSupportedException($"An unexpected type has occurred: {type.FullName}. Expected: {SupportedType.FullName}");
    }

    private static void InitializeColorNames()
    {
        PropertyInfo[] properties = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public);
        foreach (PropertyInfo property in properties)
        {
            if (!IsNamedColorProperty(property))
                continue;

            String name = property.Name;
            Color color = (Color)property.GetValue(null);
            if (NamesByColor.ContainsKey(color)) // gray, grey
                continue;
            
            ColorsByName.Add(name, color);
            NamesByColor.Add(color, name);
        }

        if (ColorsByName.Count > 0)
            ColorNames = $"{ColorNames}, {String.Join(", ", ColorsByName.Keys.OrderBy(k => k))}";
    }

    private static Boolean IsNamedColorProperty(PropertyInfo property)
    {
        if (property.IsSpecialName)
            return false;

        if (property.DeclaringType != SupportedType)
            return false;

        if (property.PropertyType != SupportedType)
            return false;

        if (!property.CanRead)
            return false;

        if (property.CanWrite)
            return false;
        
        return true;
    }
    
    public static String ToHex(Color color)
    {
        var r = (Byte)Mathf.Clamp(Mathf.RoundToInt(color.r * 255), 0, 255);
        var g = (Byte)Mathf.Clamp(Mathf.RoundToInt(color.g * 255), 0, 255);
        var b = (Byte)Mathf.Clamp(Mathf.RoundToInt(color.b * 255), 0, 255);
        var a = (Byte)Mathf.Clamp(Mathf.RoundToInt(color.a * 255), 0, 255);

        return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
    }
    
    public static Color FromHex(String str)
    {
        if (str.StartsWith("#"))
            str = str.Substring(1);
        
        if (str == String.Empty)
            throw new FormatException($"The empty string cannot be recognized as a color. The string must match on of the UnityEngine.Color names or represent the HTML sequence #RRGGBBAA.");

        if (!ColorUtility.TryParseHtmlString(str, out var color))
            throw new FormatException($"The string [{str}] cannot be recognized as a color. The string must match on of the UnityEngine.Color names or represent the HTML sequence #RRGGBBAA.");

        return color;
    }
}