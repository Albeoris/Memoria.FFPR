using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;

namespace Memoria.FFPR.Configuration;

public static class EnumTypeConverter<T> where T : struct, Enum
{
    private static readonly Type SupportedType = typeof(IReadOnlyList<T>);
    
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

        IReadOnlyList<T> list = (IReadOnlyList<T>)boxed;
        return ConvertToString(list);
    }

    public static String ConvertToString(IReadOnlyList<T> list)
    {
        if (list is null || list.Count == 0)
            return String.Empty;

        return String.Join(", ", list.Select(i=>i.ToString()));
    }

    private static Object ConvertToObject(String value, Type type)
    {
        CheckSupportedType(type);

        return ConvertToObject(value);
    }

    public static IReadOnlyList<T> ConvertToObject(String value)
    {
        if (String.IsNullOrWhiteSpace(value))
            return Array.Empty<T>();

        String[] split = value
            .Split(new[]{';', ','}, StringSplitOptions.RemoveEmptyEntries)
            .Select(v => v.Trim())
            .ToArray();

        if (split.Length == 0)
            return Array.Empty<T>();
            
        return split.Select(p=>(T)Enum.Parse(typeof(T), p)).ToArray();
    }

    private static void CheckSupportedType(Type type)
    {
        if (type != SupportedType)
            throw new NotSupportedException($"An unexpected type has occurred: {type.FullName}. Expected: {SupportedType.FullName}");
    }
}