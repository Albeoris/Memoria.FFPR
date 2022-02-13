using System;
using BepInEx.Configuration;
using Memoria.FFPR.Configuration;
using KeyCode = UnityEngine.KeyCode;
using Memoria.FFPR.BeepInEx;

namespace Memoria.FFPR.Configuration.Scopes;

public sealed class EncountersConfiguration
{
    private const String Section = "Encounters";

    public ConfigEntry<Hotkey> ToggleKey { get; }
    public ConfigEntry<Hotkey> HoldKey { get; }
    public ConfigEntry<String> ToggleAction { get; }
    public ConfigEntry<String> HoldAction { get; }

    public EncountersConfiguration(ConfigFileProvider provider)
        : this(provider.Get(Section))
    {
    }
        
    public EncountersConfiguration(ConfigFile file)
    {
        ToggleKey = file.Bind(Section, nameof(ToggleKey), new Hotkey(KeyCode.F2),
            $"Disable/Enable encounters toggle key.",
            new AcceptableHotkey(nameof(ToggleKey)));

        HoldKey = file.Bind(Section, nameof(HoldKey), Hotkey.None,
            $"Disable encounters hold key.",
            new AcceptableHotkey(nameof(HoldKey)));

        ToggleAction = file.Bind(Section, nameof(ToggleAction), "None",
            $"Disable/Enable encounters action.",
            new AcceptableValueList<String>("None", "Enter", "Cancel", "Shortcut", "Menu", "Up", "Down", "Left", "Right", "SwitchLeft", "SwitchRight", "PageUp", "PageDown", "Start"));

        HoldAction = file.Bind(Section, nameof(HoldAction), "None",
            $"Disable encounters hold action.",
            new AcceptableValueList<String>("None", "Enter", "Cancel", "Shortcut", "Menu", "Up", "Down", "Left", "Right", "SwitchLeft", "SwitchRight", "PageUp", "PageDown", "Start"));
    }

    public void CopyFrom(EncountersConfiguration other)
    {
        ToggleKey.Value = other.ToggleKey.Value;
        HoldKey.Value = other.HoldKey.Value;
        ToggleAction.Value = other.ToggleAction.Value;
        HoldAction.Value = other.HoldAction.Value;
    }
}