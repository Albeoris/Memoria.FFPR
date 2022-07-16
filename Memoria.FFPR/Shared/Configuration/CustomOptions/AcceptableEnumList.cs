using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;

namespace Memoria.FFPR.Configuration;

public sealed class AcceptableEnumList<T> : AcceptableValueBase, IAcceptableValue<IReadOnlyList<T>>
    where T : struct, Enum
{
    private readonly String _optionName;

    public AcceptableEnumList(String optionName) : base(typeof(IReadOnlyList<T>))
    {
        _optionName = optionName;
        EnumTypeConverter<T>.Init();
    }

    public override Object Clamp(Object value)
    {
        Type enumType = typeof(T);
        if (value is IReadOnlyList<T> list)
        {
            if (list.Any(i => !Enum.IsDefined(enumType, i)))
                return list.Where(i => Enum.IsDefined(enumType, i)).ToArray();
            return list;
        }

        return Array.Empty<T>();
    }

    public override Boolean IsValid(Object value)
    {
        Type enumType = typeof(T);
        if (value is IReadOnlyList<T> list)
            return list.All(i => Enum.IsDefined(enumType, i));

        return false;
    }

    public override String ToDescriptionString()
    {
        String[] names = Enum.GetNames(typeof(T));
        return $"# Acceptable values: {String.Join(", ", names)}";
    }

    public IReadOnlyList<T> FromConfig(IReadOnlyList<T> value) => value;
    public IReadOnlyList<T> ToConfig(IReadOnlyList<T> value) => value;
}