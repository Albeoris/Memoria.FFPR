using System;
using System.Linq;
using BepInEx.Configuration;

namespace Memoria.FFPR.Configuration;

public sealed class AcceptableHotkeyGroup : AcceptableValueBase
{
    private readonly String _optionName;
    private readonly Boolean _canHold;

    public AcceptableHotkeyGroup(String optionName, Boolean canHold) : base(typeof(HotkeyGroup))
    {
        _optionName = optionName;
        _canHold = canHold;
        HotkeyGroupTypeConverter.Init();
    }

    public override Object Clamp(Object value)
    {
        if (_canHold)
            return value;
        
        if (value is HotkeyGroup group)
        {
            if (group.Keys.Any(h => h.MustHeld))
                return HotkeyGroup.Create(group.Keys.Where(h => !h.MustHeld).ToArray());
        }

        return value;
    }

    public override Boolean IsValid(Object value)
    {
        return true;
    }

    public override String ToDescriptionString()
    {
        return $"# Acceptable keys: Ctrl+Alt+Shift+Key{(_canHold ? "(Hold)" : String.Empty)}: https://docs.unity3d.com/ScriptReference/KeyCode.html" +
               $"{Environment.NewLine}" +
               $"# Acceptable actions: Ctrl+Alt+[Action]+[Action]: [None], [Enter], [Cancel], [Shortcut], [Menu], [Up], [Down], [Left], [Right], [SwitchLeft], [SwitchRight], [PageUp], [PageDown], [Start]";
    }
}