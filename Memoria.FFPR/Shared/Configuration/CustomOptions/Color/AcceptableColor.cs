using System;
using BepInEx.Configuration;
using UnityEngine;
using Object = System.Object;

namespace Memoria.FFPR.Configuration;

public sealed class AcceptableColor : AcceptableValueBase, IAcceptableValue<Color>
{
    private readonly String _optionName;

    public AcceptableColor(String optionName) : base(typeof(Color))
    {
        _optionName = optionName;
        ColorTypeConverter.Init();
    }

    public override Object Clamp(Object value)
    {
        return value;
    }

    public override Boolean IsValid(Object value)
    {
        return true;
    }

    public override String ToDescriptionString()
    {
        return $"# Acceptable values: {ColorTypeConverter.ColorNames}";
    }

    public Color FromConfig(Color value) => value;
    public Color ToConfig(Color value) => value;
}