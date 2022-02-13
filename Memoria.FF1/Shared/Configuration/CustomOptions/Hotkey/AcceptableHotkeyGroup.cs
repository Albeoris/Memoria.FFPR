using System;
using BepInEx.Configuration;

namespace Memoria.FFPR.Configuration;

public sealed class AcceptableHotkeyGroup : AcceptableValueBase
{
    private readonly String _optionName;

    public AcceptableHotkeyGroup(String optionName) : base(typeof(HotkeyGroup))
    {
        _optionName = optionName;
        HotkeyGroupTypeConverter.Init();
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
        return $"# Acceptable keys: Ctrl+Alt+Shift+Key: https://docs.unity3d.com/ScriptReference/KeyCode.html" +
               $"{Environment.NewLine}" +
               $"# Acceptable actions: Ctrl+Alt+[Action]+[Action]: [None], [Enter], [Cancel], [Shortcut], [Menu], [Up], [Down], [Left], [Right], [SwitchLeft], [SwitchRight], [PageUp], [PageDown], [Start]";
    }
}