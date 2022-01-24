using System;
using BepInEx.Configuration;
using UnityEngine;
using Object = System.Object;

namespace Memoria.FFPR.Configuration;

public sealed class AcceptableHotkey : AcceptableValueBase
{
    private readonly String _optionName;

    public AcceptableHotkey(String optionName) : base(typeof(Hotkey))
    {
        _optionName = optionName;
        HotkeyTypeConverter.Init();
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
        return $"# Acceptable values: Ctrl+Alt+Shift+Key: https://docs.unity3d.com/ScriptReference/KeyCode.html";
    }
}